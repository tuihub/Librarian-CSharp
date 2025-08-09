using Consul;
using Librarian.Common;
using Librarian.Common.Models;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Librarian.Angela.Services
{
    public class PullMetadataService
    {
        private readonly ILogger _logger;
        private readonly IConsulClient _consulClient;
        private readonly SephirahContext _sephirahContext;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBusControl _busControl;

        private readonly CancellationTokenSource _cts;
        private readonly Thread _mainPullMetadataThread;
        private readonly Dictionary<(string, string), HostReceiveEndpointHandle> _platformEndpoints;

        public PullMetadataService(
            ILogger<PullMetadataService> logger,
            SephirahContext sephirahContext,
            IConsulClient consulClient,
            IServiceProvider serviceProvider,
            IBusControl busControl)
        {
            _logger = logger;
            _consulClient = consulClient;
            _sephirahContext = sephirahContext;
            _serviceProvider = serviceProvider;
            _busControl = busControl;

            _cts = new CancellationTokenSource();
            _mainPullMetadataThread = new Thread(() => MainPullMetadataThread(_cts.Token));
            _platformEndpoints = new Dictionary<(string, string), HostReceiveEndpointHandle>();
        }

        public void Start()
        {
            _mainPullMetadataThread.Start();

            // Initialize static Porter instances
            InitializeStaticPorterInstances();
        }

        public void Cancel()
        {
            _cts.Cancel();
        }

        public void EnablePorter(string porterServiceId)
        {
            _logger.LogInformation("Porter service {ServiceId} enabled", porterServiceId);
        }

        // Initialize static Porter instances from configuration
        private void InitializeStaticPorterInstances()
        {
            if (_sephirahContext.StaticPorterInstances == null || _sephirahContext.StaticPorterInstances.Count == 0)
            {
                _logger.LogInformation("No static Porter instances configured");
                return;
            }

            _logger.LogInformation("Initializing {Count} static Porter instances", _sephirahContext.StaticPorterInstances.Count);

            // For each static Porter instance, create endpoints for its tags
            foreach (var porter in _sephirahContext.StaticPorterInstances)
            {
                _logger.LogInformation("Initializing static Porter: Id={Id}, Url={Url}, Tags={Tags}",
                    porter.Id, porter.Url, string.Join(",", porter.Tags));

                // Create endpoints for each tag
                foreach (var tag in porter.Tags)
                {
                    _logger.LogInformation("Creating endpoint for static Porter ({Id}, {Tag})", porter.Id, tag);

                    try
                    {
                        // 为静态Porter的每个Tag创建一个终端点
                        var handle = _busControl.ConnectReceiveEndpoint(tag, _ => { });

                        // 存储终端点句柄以便管理
                        _platformEndpoints[(porter.Id, tag)] = handle;
                        _logger.LogInformation("Successfully created endpoint for static Porter ({Id}, {Tag})", porter.Id, tag);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to create endpoint for static Porter ({Id}, {Tag})", porter.Id, tag);
                    }
                }
            }
        }

        private async void MainPullMetadataThread(CancellationToken ct)
        {
            _logger.LogInformation("PullMetadataService started");
            var consulQueryOptions = new QueryOptions
            {
                WaitTime = TimeSpan.FromSeconds(120)
            };
            var serviceName = "porter";
            while (true)
            {
                QueryResult<ServiceEntry[]> response;
                try
                {
                    response = await _consulClient.Health.Service(serviceName, null, true, consulQueryOptions, ct);
                }
                catch (TaskCanceledException)
                {
                    // stop all endpoints
                    foreach (var endpoint in _platformEndpoints.Values)
                    {
                        await endpoint.StopAsync();
                    }
                    _platformEndpoints.Clear();
                    break;
                }
                _logger.LogInformation($"Got {serviceName}, WaitIndex = {consulQueryOptions.WaitIndex}, LastIndex = {response.LastIndex}");
                if (response.LastIndex != consulQueryOptions.WaitIndex)
                {
                    consulQueryOptions.WaitIndex = response.LastIndex;

                    // update the endpoints for dynamic Porter instances from Consul
                    // 只处理不是静态Porter的终端点
                    var existingDynamicEndpoints = _platformEndpoints.Keys
                        .Where(k => !_sephirahContext.StaticPorterInstances.Any(p => p.Id == k.Item1))
                        .ToList();

                    var serviceTags = response.Response
                        .Select(x => x.Service)
                        .SelectMany(s => s.Tags,
                            (s, t) => (s.ID, t))
                        .ToList();

                    var endpointsToRemove = existingDynamicEndpoints.Except(serviceTags).ToList();
                    foreach (var endpointToRemove in endpointsToRemove)
                    {
                        _logger.LogInformation($"Stopping endpoint for ({endpointToRemove.Item1}, {endpointToRemove.Item2})");
                        var endpoint = _platformEndpoints[endpointToRemove];
                        await endpoint.StopAsync();
                        _platformEndpoints.Remove(endpointToRemove);
                    }

                    var endpointsToAdd = serviceTags.Except(existingDynamicEndpoints).ToList();
                    foreach (var endpointToAdd in endpointsToAdd)
                    {
                        _logger.LogInformation($"Creating endpoint for ({endpointToAdd.Item1}, {endpointToAdd.Item2})");
                        var platform = endpointToAdd.Item2;

                        // 不在这里配置消费者，使用空的配置函数
                        // 消费者已经在StartUp.cs的RegisterConsumers中注册
                        var handle = _busControl.ConnectReceiveEndpoint(platform, _ => { });

                        _platformEndpoints[endpointToAdd] = handle;
                    }

                    // 更新SephirahContext中的动态Porter服务
                    _sephirahContext.PorterServices = response.Response.Select(x => x.Service).ToList();
                }
            }
        }

        public async Task SendAppIdMQAsync(string platform, string appId, bool updateInternalName = false)
        {
            var message = new AppIdMQ
            {
                AppId = appId,
            };

            try
            {
                // send to platform queue
                var sendEndpoint = await _busControl.GetSendEndpoint(new Uri($"queue:{platform}"));
                await sendEndpoint.Send(message);
                _logger.LogDebug("Sent message to platform {Platform}: {message}", platform, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message to platform {Platform}: {message}", platform, message);
            }
        }
    }
}
