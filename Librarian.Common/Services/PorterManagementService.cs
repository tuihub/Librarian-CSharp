using Consul;
using Librarian.Common.Configs;
using Librarian.Common.Constants;
using Librarian.Common.Helpers;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Services;

public class PorterManagementService
{
    private readonly ILogger<PorterManagementService> _logger;
    private readonly IConsulClient _consulClient;
    private readonly SystemConfig _systemConfig;
    private readonly ConsulConfig _consulConfig;

    // key: porterId
    private readonly ConcurrentDictionary<string, PorterInstance> _porterInstances = new();
    // key: porterId:feature
    private readonly ConcurrentDictionary<string, PorterInstanceFeatureInfo> _porterInstanceFeatureInfos = new();

    private readonly CancellationTokenSource _cts = new();
    private readonly Thread? _consulMonitorThread;

    public PorterManagementService(ILogger<PorterManagementService> logger, IConsulClient consulClient, SystemConfig systemConfig, ConsulConfig consulConfig)
    {
        _logger = logger;
        _consulClient = consulClient;
        _systemConfig = systemConfig;
        _consulConfig = consulConfig;

        InitializeStaticPorterInstances();

        if (_consulConfig.IsEnabled)
        {
            _consulMonitorThread = new Thread(() => ConsulMonitorThread(_cts.Token))
            {
                IsBackground = true
            };
            _consulMonitorThread.Start();
        }
    }

    public void EnablePorterInstance(string porterId, FeatureSummary featureSummary, string region)
    {
        if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
        {
            _logger.LogError("Porter instance not found: {PorterId}", porterId);
            return;
        }

        lock (porterInstance.Lock)
        {
            porterInstance.IsEnabled = true;
            var newFeatures = FeatureSummaryToPorterInterfaces(featureSummary);
            UpdatePorterInterfaceStatus(porterId, CollectionHelper.CompareCollections(
                porterInstance.Features,
                newFeatures,
                f => f,
                (l, r) => l == r));
            porterInstance.ClearFeatures();
            porterInstance.AddRangeFeatures(newFeatures);
            porterInstance.Region = region;
            foreach (var feature in newFeatures)
            {
                _porterInstanceFeatureInfos.TryAdd(porterId + ":" + feature, new PorterInstanceFeatureInfo(porterId, region, feature, true, 0, 1, DateTime.UtcNow));
            }
        }
    }

    public void DisablePorterInstance(string porterId)
    {
        if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
        {
            _logger.LogError("Porter instance not found: {PorterId}", porterId);
            return;
        }

        lock (porterInstance.Lock)
        {
            porterInstance.IsEnabled = false;
            // create a copy of features to avoid modifying the collection while iterating
            var features = porterInstance.Features.ToList();
            foreach (var feature in features)
            {
                _porterInstanceFeatureInfos.TryRemove($"{porterId}:{feature}", out _);
            }
            UpdatePorterInterfaceStatus(porterId, ([], [], []));
            porterInstance.ClearFeatures();
        }
    }

    public async Task<PorterInstance?> AcquirePorterInstanceAsync(string feature, string? region = null, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Acquiring porter instance for feature: {Feature} in region: {Region}", feature, region);

        const int maxRetries = 3;
        for (int retry = 0; retry < maxRetries; retry++)
        {
            if (retry > 0)
            {
                _logger.LogDebug("Retrying to acquire porter instance for feature: {Feature} in region: {Region}, attempt: {Attempt}", feature, region, retry + 1);
            }

            // create a copy of candidateInfos to avoid modifying the collection while iterating
            var candidateInfos = _porterInstanceFeatureInfos.Values
                .Where(info => info.IsEnabled && info.Feature == feature)
                .Where(info => _porterInstances.TryGetValue(info.InstanceId, out var inst) && inst.IsHealthy)
                .ToList();

            // region filter
            if (!string.IsNullOrWhiteSpace(region))
            {
                if (region.StartsWith('!'))
                {
                    candidateInfos = candidateInfos.Where(info => info.Region != region[1..]).ToList();
                }
                else
                {
                    candidateInfos = candidateInfos.Where(info => info.Region == region).ToList();
                }
            }

            // sort by remaining concurrency and last access time
            PorterInstanceFeatureInfo? selectedInfo = null;
            // NOTE: potential minor race condition here
            selectedInfo = candidateInfos
                .OrderByDescending(info => info.RemainingConcurrency)
                .ThenBy(info => info.LastAccessTime)
                .FirstOrDefault();

            if (selectedInfo is null)
            {
                _logger.LogWarning("No available porter instance found for feature: {Feature} in region: {Region}", feature, region);
                return null;
            }

            if (!_porterInstances.TryGetValue(selectedInfo.InstanceId, out var porterInstance))
            {
                _logger.LogWarning("Porter instance not found after sorting: {InstanceId}, feature: {Feature}, region: {Region}, retry: {Retry}", selectedInfo.InstanceId, feature, region, retry);
                continue;
            }

            // check the instance status and try to acquire the semaphore in the lock
            bool acquired = false;
            lock (porterInstance.Lock)
            {
                // Recheck the instance status to avoid race conditions
                if (!porterInstance.IsEnabled || !porterInstance.IsHealthy || !porterInstance.Features.Contains(feature))
                {
                    _logger.LogWarning("Porter instance status changed after sorting: {InstanceId}, feature: {Feature}, region: {Region}, retry: {Retry}", selectedInfo.InstanceId, feature, region, retry);
                    continue;
                }

                // check if the semaphore exists
                if (!porterInstance.FeatureSemaphores.TryGetValue(feature, out var semaphore))
                {
                    _logger.LogWarning("Porter instance feature semaphore not found: {InstanceId}, feature: {Feature}, region: {Region}, retry: {Retry}", selectedInfo.InstanceId, feature, region, retry);
                    continue;
                }

                // get the cancellation token
                if (!porterInstance.FeatureCancellationTokenSources.TryGetValue(feature, out var cts))
                {
                    _logger.LogWarning("Porter instance feature cancellation token source not found: {InstanceId}, feature: {Feature}, region: {Region}, retry: {Retry}", selectedInfo.InstanceId, feature, region, retry);
                    continue;
                }

                // use the combined cancellation token
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

                try
                {
                    // try to acquire the semaphore immediately (without waiting)
                    if (semaphore.Wait(0))
                    {
                        acquired = true;
                        porterInstance.FeatureLastAccessTimes[feature] = DateTime.UtcNow;
                    }
                }
                catch
                {
                    // ignore the exception and retry
                    _logger.LogWarning("Failed to acquire semaphore immediately: {InstanceId}, feature: {Feature}, region: {Region}, retry: {Retry}", selectedInfo.InstanceId, feature, region, retry);
                }
            }

            // if not acquired immediately, wait for the semaphore in the lock
            if (!acquired)
            {
                try
                {
                    // get the cancellation token
                    if (!porterInstance.FeatureCancellationTokenSources.TryGetValue(feature, out var cts))
                    {
                        _logger.LogWarning("Porter instance feature cancellation token source not found: {InstanceId}, feature: {Feature}, region: {Region}, retry: {Retry}", selectedInfo.InstanceId, feature, region, retry);
                        continue;
                    }

                    // use the combined cancellation token
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

                    // wait for the semaphore in the lock
                    await porterInstance.FeatureSemaphores[feature].WaitAsync(linkedCts.Token);

                    // recheck the instance status in the lock
                    lock (porterInstance.Lock)
                    {
                        if (!porterInstance.IsEnabled || !porterInstance.IsHealthy || !porterInstance.Features.Contains(feature))
                        {
                            // status changed, release the semaphore and retry
                            _logger.LogWarning("Porter instance status changed after waiting: {InstanceId}, feature: {Feature}, region: {Region}, retry: {Retry}", selectedInfo.InstanceId, feature, region, retry);
                            porterInstance.FeatureSemaphores[feature].Release();
                            continue;
                        }
                        porterInstance.FeatureLastAccessTimes[feature] = DateTime.UtcNow;
                        acquired = true;
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Acquisition cancelled for feature: {Feature} in region: {Region}, retry: {Retry}", feature, region, retry);
                    continue;
                }
            }

            if (acquired)
            {
                // update the concurrency information
                lock (selectedInfo.Lock)
                {
                    selectedInfo.RemainingConcurrency--;
                    selectedInfo.LastAccessTime = DateTime.UtcNow;
                    selectedInfo.CurrentConcurrency++;
                }

                _logger.LogInformation("Acquired porter instance: {InstanceId} for feature: {Feature} in region: {Region}", selectedInfo.InstanceId, feature, region);
                return porterInstance;
            }
        }

        _logger.LogError("Failed to acquire porter instance after {MaxRetries} retries for feature: {Feature} in region: {Region}", maxRetries, feature, region);
        return null;
    }

    public void ReleasePorterInstance(string porterId, string feature)
    {
        if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
        {
            _logger.LogError("Porter instance not found: {PorterId}", porterId);
            return;
        }

        _logger.LogDebug("Releasing porter instance: {InstanceId} for feature: {Feature} in region: {Region}", porterId, feature, porterInstance.Region);

        // release the semaphore
        lock (porterInstance.Lock)
        {
            if (porterInstance.FeatureSemaphores.TryGetValue(feature, out var semaphore))
            {
                semaphore.Release();
            }
        }

        // update the concurrency information
        var featureInfoKey = $"{porterId}:{feature}";
        if (_porterInstanceFeatureInfos.TryGetValue(featureInfoKey, out var featureInfo))
        {
            lock (featureInfo.Lock)
            {
                featureInfo.RemainingConcurrency++;
                featureInfo.CurrentConcurrency--;
            }
        }

        _logger.LogInformation("Released porter instance: {InstanceId} for feature: {Feature} in region: {Region}", porterId, feature, porterInstance.Region);
    }

    public void IncreasePorterInstanceFeatureConcurrency(string porterId, string feature)
    {
        if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
        {
            _logger.LogError("Porter instance not found: {PorterId}", porterId);
            return;
        }

        _logger.LogDebug("Increasing porter instance: {InstanceId} for feature: {Feature} in region: {Region}", porterId, feature, porterInstance.Region);

        // release an extra semaphore permit
        lock (porterInstance.Lock)
        {
            if (porterInstance.FeatureSemaphores.TryGetValue(feature, out var semaphore))
            {
                semaphore.Release();
            }
        }

        // update the concurrency information
        var featureInfoKey = $"{porterId}:{feature}";
        if (_porterInstanceFeatureInfos.TryGetValue(featureInfoKey, out var featureInfo))
        {
            lock (featureInfo.Lock)
            {
                featureInfo.RemainingConcurrency++;
            }
        }

        _logger.LogInformation("Increased porter instance: {InstanceId} for feature: {Feature} in region: {Region}", porterId, feature, porterInstance.Region);
    }

    public async Task DecreasePorterInstanceFeatureConcurrencyAsync(string porterId, string feature)
    {
        if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
        {
            _logger.LogError("Porter instance not found: {PorterId}", porterId);
            return;
        }

        _logger.LogDebug("Decreasing porter instance: {InstanceId} for feature: {Feature} in region: {Region}", porterId, feature, porterInstance.Region);

        var featureInfoKey = $"{porterId}:{feature}";
        if (!_porterInstanceFeatureInfos.TryGetValue(featureInfoKey, out var featureInfo))
        {
            _logger.LogError("Porter instance feature info not found: {FeatureInfoKey}", featureInfoKey);
            return;
        }

        // check if the concurrency can be decreased
        SemaphoreSlim? semaphore = null;
        bool shouldDecrease = false;

        lock (porterInstance.Lock)
        {
            if (porterInstance.FeatureSemaphores.TryGetValue(feature, out semaphore))
            {
                lock (featureInfo.Lock)
                {
                    // check if the total concurrency capacity is greater than 1
                    if (semaphore.CurrentCount + featureInfo.CurrentConcurrency > 1)
                    {
                        shouldDecrease = true;
                    }
                }
            }
        }

        // wait for the semaphore in the lock
        if (shouldDecrease && semaphore != null)
        {
            await semaphore.WaitAsync();

            // update the concurrency information
            lock (featureInfo.Lock)
            {
                featureInfo.RemainingConcurrency--;
            }

            _logger.LogInformation("Decreased porter instance: {InstanceId} for feature: {Feature} in region: {Region}", porterId, feature, porterInstance.Region);
        }
        else
        {
            _logger.LogWarning("Cannot decrease concurrency for porter instance: {InstanceId} feature: {Feature} - would result in zero capacity", porterId, feature);
        }
    }

    private void UpdatePorterInterfaceStatus(string porterId, (ICollection<string> ToRemove, ICollection<string> ToAdd, ICollection<(string OldItem, string NewItem)> ToUpdate) featureChanges)
    {
        foreach (var feature in featureChanges.ToRemove)
        {
            if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
            {
                _logger.LogError("Porter instance not found: {PorterId}", porterId);
                continue;
            }
            lock (porterInstance.Lock)
            {
                porterInstance.RemoveFeature(feature);
            }
        }
        foreach (var feature in featureChanges.ToAdd)
        {
            if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
            {
                _logger.LogError("Porter instance not found: {PorterId}", porterId);
                continue;
            }
            lock (porterInstance.Lock)
            {
                porterInstance.FeatureSemaphores.TryAdd(feature, new SemaphoreSlim(1, 1));
            }
        }
        if (featureChanges.ToUpdate.Count != 0)
        {
            _logger.LogError("ToUpdate should be empty");
        }
    }

    private static List<string> FeatureSummaryToPorterInterfaces(FeatureSummary featureSummary)
    {
        var porterInterfaces = new List<string>();
        // account platform
        foreach (var accountPlatform in featureSummary.AccountPlatforms)
        {
            porterInterfaces.Add($"{PorterFeature.AccountPlatform}:{accountPlatform}");
        }
        // app info source
        foreach (var appInfoSource in featureSummary.AppInfoSources)
        {
            porterInterfaces.Add($"{PorterFeature.AppInfoSource}:{appInfoSource}");
        }
        // feed source
        foreach (var feedSource in featureSummary.FeedSources)
        {
            porterInterfaces.Add($"{PorterFeature.FeedSource}:{feedSource}");
        }
        // notify destination
        foreach (var notifyDestination in featureSummary.NotifyDestinations)
        {
            porterInterfaces.Add($"{PorterFeature.NotifyDestination}:{notifyDestination}");
        }
        // feed item action
        foreach (var feedItemAction in featureSummary.FeedItemActions)
        {
            porterInterfaces.Add($"{PorterFeature.FeedItemAction}:{feedItemAction}");
        }
        // feed getter
        foreach (var feedGetter in featureSummary.FeedGetters)
        {
            porterInterfaces.Add($"{PorterFeature.FeedGetter}:{feedGetter}");
        }
        // feed setter
        foreach (var feedSetter in featureSummary.FeedSetters)
        {
            porterInterfaces.Add($"{PorterFeature.FeedSetter}:{feedSetter}");
        }
        return porterInterfaces;
    }

    private void InitializeStaticPorterInstances()
    {
        foreach (var porterInstance in _systemConfig.StaticPorterInstances)
        {
            _porterInstances.TryAdd(porterInstance.Id, new PorterInstance(porterInstance.Id, porterInstance.Url, [], string.Empty, false, true));
        }
    }

    private async void ConsulMonitorThread(CancellationToken ct)
    {
        _logger.LogInformation("PorterManagementService Consul monitor started");
        var queryOptions = new QueryOptions
        {
            WaitTime = TimeSpan.FromSeconds(120)
        };
        const string serviceName = "porter";
        while (true)
        {
            QueryResult<ServiceEntry[]> response;
            try
            {
                response = await _consulClient.Health.Service(serviceName, null, true, queryOptions, ct);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("PorterManagementService Consul monitor cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying Consul for porter services");
                // backoff to avoid busy loop
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), ct);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                continue;
            }

            if (response.LastIndex == queryOptions.WaitIndex)
            {
                // no changes
                continue;
            }
            queryOptions.WaitIndex = response.LastIndex;

            var currentServiceIds = response.Response.Select(se => se.Service.ID).ToHashSet();

            // handle additions & health updates
            foreach (var entry in response.Response)
            {
                var service = entry.Service;
                var porterId = service.ID;
                var url = $"http://{service.Address}:{service.Port}";
                var healthy = entry.Checks.All(c => c.Status == HealthStatus.Passing);

                if (!_porterInstances.ContainsKey(porterId))
                {
                    var instance = new PorterInstance(porterId, url, [], string.Empty, healthy, healthy);
                    _porterInstances[porterId] = instance;
                    _logger.LogInformation("Discovered new porter instance {PorterId} (Healthy={Healthy})", porterId, healthy);
                }
                else
                {
                    var instance = _porterInstances[porterId];
                    lock (instance.Lock)
                    {
                        // update Url if changed
                        instance.Url = url;
                        // health status change
                        if (healthy && !instance.IsHealthy)
                        {
                            instance.IsHealthy = healthy;
                            _logger.LogInformation("Porter instance {PorterId} is now healthy", porterId);
                        }
                        else if (!healthy && instance.IsHealthy)
                        {
                            instance.IsHealthy = healthy;
                            _logger.LogInformation("Porter instance {PorterId} became unhealthy", porterId);
                        }
                    }
                }
            }

            // handle removals (only for dynamic instances)
            var dynamicExistingIds = _porterInstances.Keys
                .Where(id => !_systemConfig.StaticPorterInstances.Any(p => p.Id == id))
                .ToList();
            var removedIds = dynamicExistingIds.Except(currentServiceIds).ToList();
            foreach (var removed in removedIds)
            {
                _logger.LogInformation("Porter instance {PorterId} removed from Consul, disabling", removed);
                DisablePorterInstance(removed);
            }
        }
    }
}

class PorterInstanceFeatureInfo(string instanceId, string region, string feature, bool isEnabled, int currentConcurrency, int remainingConcurrency, DateTime lastAccessTime)
{
    private readonly object _lock = new();
    public object Lock => _lock;

    public string InstanceId { get; } = instanceId;
    public bool IsEnabled { get; } = isEnabled;
    public string Region { get; } = region;
    public string Feature { get; } = feature;

    public int CurrentConcurrency { get; set; } = currentConcurrency;
    public int RemainingConcurrency { get; set; } = remainingConcurrency;
    public DateTime LastAccessTime { get; set; } = lastAccessTime;
}

public class PorterInstance(string id, string url, List<string> features, string region, bool isEnabled, bool isHealthy)
{
    private readonly object _lock = new();
    public object Lock => _lock;

    public string Id { get; set; } = id;
    public string Url { get; set; } = url;
    public List<string> Features { get; } = features;
    public string Region { get; set; } = region;
    public bool IsEnabled { get; set; } = isEnabled;
    public bool IsHealthy { get; set; } = isHealthy;
    public ConcurrentDictionary<string, SemaphoreSlim> FeatureSemaphores { get; } = new();
    public ConcurrentDictionary<string, CancellationTokenSource> FeatureCancellationTokenSources { get; } = new();
    public ConcurrentDictionary<string, DateTime> FeatureLastAccessTimes { get; } = new();

    public void ClearFeatures()
    {
        Features.Clear();
        FeatureSemaphores.Clear();
        foreach (var cancellationTokenSource in FeatureCancellationTokenSources.Values)
        {
            cancellationTokenSource.Cancel();
        }
        FeatureCancellationTokenSources.Clear();
        FeatureLastAccessTimes.Clear();
    }
    public void AddRangeFeatures(IEnumerable<string> features)
    {
        Features.AddRange(features);
        foreach (var feature in features)
        {
            AddFeature(feature);
        }
    }
    public void RemoveFeature(string feature)
    {
        Features.Remove(feature);
        FeatureSemaphores.TryRemove(feature, out _);
        if (FeatureCancellationTokenSources.TryGetValue(feature, out CancellationTokenSource? value))
        {
            value.Cancel();
            FeatureCancellationTokenSources.TryRemove(feature, out _);
        }
        FeatureLastAccessTimes.TryRemove(feature, out _);
    }
    public void AddFeature(string feature)
    {
        Features.Add(feature);
        FeatureSemaphores.TryAdd(feature, new SemaphoreSlim(1, 1));
        FeatureCancellationTokenSources.TryAdd(feature, new CancellationTokenSource());
        FeatureLastAccessTimes.TryAdd(feature, DateTime.UtcNow);
    }
}
