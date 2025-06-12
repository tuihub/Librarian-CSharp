using System;
using System.Collections.Concurrent;
using Consul;
using Librarian.Common.Configs;
using Librarian.Common.Constants;
using Librarian.Common.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Services;

public class PorterManagementService
{
    private readonly ILogger<PorterManagementService> _logger;
    private readonly IConsulClient _consulClient;
    private readonly SystemConfig _systemConfig;
    
    // key: porterId
    private readonly ConcurrentDictionary<string, PorterInstance> _porterInstances = new();
    // key: porterId:feature
    private readonly ConcurrentDictionary<string, PorterInstanceFeatureInfo> _porterInstanceFeatureInfos = new();

    public PorterManagementService(ILogger<PorterManagementService> logger, IConsulClient consulClient, SystemConfig systemConfig)
    {
        _logger = logger;
        _consulClient = consulClient;
        _systemConfig = systemConfig;

        InitializeStaticPorterInstances();
    }

    public void EnablePorterInstance(string porterId, FeatureSummary featureSummary, string region)
    {
        if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
        {
            _logger.LogError("Porter instance not found: {PorterId}", porterId);
            return;
        }

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
            _porterInstanceFeatureInfos.TryAdd(porterId + ":" + feature, new PorterInstanceFeatureInfo(porterId, region, feature, true, 1, DateTime.UtcNow));
        }
    }

    public void DisablePorterInstance(string porterId)
    {
        if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
        {
            _logger.LogError("Porter instance not found: {PorterId}", porterId);
            return;
        }
        porterInstance.IsEnabled = false;
        var features = porterInstance.Features;
        foreach (var feature in features)
        {
            _porterInstanceFeatureInfos.TryRemove($"{porterId}:{feature}", out _);
        }
        UpdatePorterInterfaceStatus(porterId, ([], [], []));
        porterInstance.ClearFeatures();
    }

    public async Task<PorterInstance?> AcquirePorterInstanceAsync(string feature, string region)
    {
        var porterInstanceFeatureInfos = _porterInstanceFeatureInfos.Where(x => x.Value.IsEnabled && x.Value.Feature == feature);
        if (region.StartsWith('!'))
        {
            porterInstanceFeatureInfos = porterInstanceFeatureInfos.Where(x => x.Value.Region != region[1..]);
        }
        else
        {
            porterInstanceFeatureInfos = porterInstanceFeatureInfos.Where(x => x.Value.Region == region);
        }
        var instanceId = porterInstanceFeatureInfos
            .OrderByDescending(x => x.Value.RemainingConcurrency)
            .ThenBy(x => x.Value.LastAccessTime)
            .FirstOrDefault()
            .Key;
        if (instanceId is null)
        {
            _logger.LogWarning("No available porter instance found for feature: {Feature} in region: {Region}", feature, region);
            return null;
        }
        var porterInstance = _porterInstances[instanceId];
        var porterInstanceFeatureInfo = _porterInstanceFeatureInfos[instanceId + ":" + feature];
        // wait for the feature semaphore
        await porterInstance.FeatureSemaphores[feature].WaitAsync();
        porterInstance.FeatureLastAccessTimes[feature] = DateTime.UtcNow;
        // update porterInstanceFeatureInfo
        porterInstanceFeatureInfo.RemainingConcurrency--;
        porterInstanceFeatureInfo.LastAccessTime = DateTime.UtcNow;
        _logger.LogInformation("Acquired porter instance: {InstanceId} for feature: {Feature} in region: {Region}", instanceId, feature, region);
        return porterInstance;
    }

    public void ReleasePorterInstance(string porterId, string feature)
    {
        if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
        {
            _logger.LogError("Porter instance not found: {PorterId}", porterId);
            return;
        }
        porterInstance.FeatureSemaphores[feature].Release();
        _porterInstanceFeatureInfos[$"{porterId}:{feature}"].RemainingConcurrency++;
        _logger.LogInformation("Released porter instance: {InstanceId} for feature: {Feature} in region: {Region}", porterId, feature, porterInstance.Region);
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
            porterInstance.RemoveFeature(feature);
        }
        foreach (var feature in featureChanges.ToAdd)
        {
            if (!_porterInstances.TryGetValue(porterId, out PorterInstance? porterInstance))
            {
                _logger.LogError("Porter instance not found: {PorterId}", porterId);
                continue;
            }
            porterInstance.FeatureSemaphores.TryAdd(feature, new SemaphoreSlim(1, 1));
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
            _porterInstances.TryAdd(porterInstance.Id, new PorterInstance(porterInstance.Id, porterInstance.Url, [], string.Empty, false));
        }
    }
}

class PorterInstanceFeatureInfo(string instanceId, string region, string feature, bool isEnabled, int remainingConcurrency, DateTime lastAccessTime)
{
    public string InstanceId { get; } = instanceId;
    public bool IsEnabled { get; } = isEnabled;
    public string Region { get; } = region;
    public string Feature { get; } = feature;
    public int RemainingConcurrency { get; set; } = remainingConcurrency;
    public DateTime LastAccessTime { get; set; } = lastAccessTime;
}

public class PorterInstance(string id, string url, List<string> features, string region, bool isEnabled)
{
    private readonly object _lock = new();

    public string Id { get; set; } = id;
    public string Url { get; set; } = url;
    public List<string> Features { get; } = features;
    public string Region { get; set; } = region;
    public bool IsEnabled { get; set; } = isEnabled;
    public ConcurrentDictionary<string, SemaphoreSlim> FeatureSemaphores { get; } = new();
    public ConcurrentDictionary<string, CancellationTokenSource> FeatureCancellationTokenSources { get; } = new();
    public ConcurrentDictionary<string, DateTime> FeatureLastAccessTimes { get; } = new();

    public void ClearFeatures()
    {
        lock (_lock)
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
    }
    public void AddRangeFeatures(IEnumerable<string> features)
    {
        lock (_lock)
        {
            Features.AddRange(features);
            foreach (var feature in features)
            {
                AddFeature(feature);
            }
        }
    }
    public void RemoveFeature(string feature)
    {
        lock (_lock)
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
    }
    public void AddFeature(string feature)
    {
        lock (_lock)
        {
            Features.Add(feature);
            FeatureSemaphores.TryAdd(feature, new SemaphoreSlim(1, 1));
            FeatureCancellationTokenSources.TryAdd(feature, new CancellationTokenSource());
            FeatureLastAccessTimes.TryAdd(feature, DateTime.UtcNow);
        }
    }
}
