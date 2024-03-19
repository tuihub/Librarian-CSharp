using Consul;
using Grpc.Net.Client;
using Librarian.Angela.Services.Workers;
using Librarian.Common;
using Librarian.Common.Contracts;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Services
{
    public class PullMetadataService
    {
        private readonly ILogger _logger;
        private readonly IConsulClient _consulClient;
        private readonly SephirahContext _sephirahContext;
        private readonly IServiceProvider _serviceProvider;

        private readonly CancellationTokenSource _cts;
        private readonly Thread _mainPullMetadataThread;
        private readonly Dictionary<(string, string), PullAppInfoMetadataWorker> _pullAppInfoMetadataWorkers;

        public PullMetadataService(ILogger<PullMetadataService> logger, SephirahContext sephirahContext, IConsulClient consulClient, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _consulClient = consulClient;
            _sephirahContext = sephirahContext;
            _serviceProvider = serviceProvider;

            _cts = new CancellationTokenSource();
            _mainPullMetadataThread = new Thread(() => MainPullMetadataThread(_cts.Token));
            _pullAppInfoMetadataWorkers = new Dictionary<(string, string), PullAppInfoMetadataWorker>();
        }

        public void Start()
        {
            _mainPullMetadataThread.Start();
        }
        public void Cancel()
        {
            _cts.Cancel();
        }
        public void EnablePorter(string porterServiceId)
        {
            _pullAppInfoMetadataWorkers.Where(x => x.Key.Item1 == porterServiceId).ToList().ForEach(x => x.Value.Start());
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
                    foreach (var worker in _pullAppInfoMetadataWorkers.Values)
                    {
                        worker.Cancel();
                    }
                    break;
                }
                _logger.LogInformation($"Got {serviceName}, WaitIndex = {consulQueryOptions.WaitIndex}, LastIndex = {response.LastIndex}");
                if (response.LastIndex != consulQueryOptions.WaitIndex)
                {
                    consulQueryOptions.WaitIndex = response.LastIndex;

                    // update workers
                    var existingWorkers = _pullAppInfoMetadataWorkers.Keys.ToList();
                    var serviceTags = response.Response
                        .Select(x => x.Service)
                        .SelectMany(s => s.Tags,
                            (s, t) => (s.ID, t))
                        .ToList();
                    var workersToRemove = existingWorkers.Except(serviceTags).ToList();
                    foreach (var workerToRemove in workersToRemove)
                    {
                        _logger.LogInformation($"Cancelling worker for ({workerToRemove.Item1}, {workerToRemove.Item2})");
                        var worker = _pullAppInfoMetadataWorkers[workerToRemove];
                        worker.Cancel();
                        _pullAppInfoMetadataWorkers.Remove(workerToRemove);
                    }
                    var workersToAdd = serviceTags.Except(existingWorkers).ToList();
                    foreach (var workerToAdd in workersToAdd)
                    {
                        _logger.LogInformation($"Adding worker for ({workerToAdd.Item1}, {workerToAdd.Item2})");
                        var service = response.Response.Where(x => x.Service.ID == workerToAdd.Item1).First().Service;
                        var serviceUrl = $"http://{service.Address}:{service.Port}";
                        var worker = new PullAppInfoMetadataWorker(
                            _serviceProvider,
                            GrpcChannel.ForAddress(serviceUrl),
                            workerToAdd.Item1,
                            workerToAdd.Item2);
                        _pullAppInfoMetadataWorkers[workerToAdd] = worker;
                    }

                    // update SephirahContext
                    _sephirahContext.PorterServices = response.Response.Select(x => x.Service).ToList();
                }
            }
        }
    }
}
