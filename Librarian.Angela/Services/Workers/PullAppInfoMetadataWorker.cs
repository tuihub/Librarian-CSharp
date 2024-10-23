using Consul;
using Grpc.Core;
using Grpc.Net.Client;
using Librarian.Common.Contracts;
using Librarian.Common.Models;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Porter.V1;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Angela.Services.Workers
{
    public class PullAppInfoMetadataWorker
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly GrpcChannel _grpcChannel;

        private readonly ILogger _logger;
        private readonly IMessageQueueService _messageQueueService;
        private readonly CancellationTokenSource _cts;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private bool _isStarted = false;

        public string ServiceId { get; set; } = null!;
        public string Platform { get; set; } = null!;
        public PullAppInfoMetadataWorker(IServiceProvider serviceProvider, GrpcChannel grpcChannel, string serviceId, string platform)
        {
            _serviceProvider = serviceProvider;
            _grpcChannel = grpcChannel;

            ServiceId = serviceId;
            Platform = platform;

            _logger = _serviceProvider.GetRequiredService<ILogger<PullAppInfoMetadataWorker>>();
            _messageQueueService = _serviceProvider.GetRequiredService<IMessageQueueService>();
            _cts = new CancellationTokenSource();
            _retryPolicy = Polly.Policy
                .Handle<RpcException>(ex => ex.StatusCode == StatusCode.ResourceExhausted)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(10 * Math.Pow(2, retryAttempt)),
                    (ex, time, retryCount, context) =>
                    {
                        if (retryCount > 0)
                        {
                            _logger.LogWarning(ex, "Failed to connect to porter service. Retrying in {time}", time);
                        }
                    });
            _connection = _serviceProvider.GetRequiredService<IConnection>();
            _channel = _connection.CreateModel();
        }

        public void Start()
        {
            if (!_isStarted)
            {
                Worker(_cts.Token);
                _isStarted = true;
            }
        }

        public void Cancel()
        {
            _logger.LogInformation($"Cancelling worker ({ServiceId}, {Platform})");
            if (_isStarted) { _messageQueueService.UnsubscribeQueue(_channel, Platform); }
            _cts.Cancel();
        }

        private void Worker(CancellationToken ct)
        {
            _logger.LogInformation($"Worker for ({ServiceId}, {Platform}) started");
            _channel.BasicQos(0, 1, false);
            _channel.QueueDeclare(queue: Platform,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
            var workerFunc = new Func<AppIdMQ, CancellationToken, Task>(WorkerFunc);
            _messageQueueService.SubscribeQueue(_channel, Platform, workerFunc, ct);
        }

        private async Task WorkerFunc(AppIdMQ appIdMq, CancellationToken ct = default)
        {
            _logger.LogDebug($"Worker ({ServiceId}, {Platform}) got message: {appIdMq}");
            var client = new LibrarianPorterService.LibrarianPorterServiceClient(_grpcChannel);
            TuiHub.Protos.Librarian.V1.AppInfo? appInfoResp = null;
            try
            {
                await _retryPolicy.ExecuteAsync(async (ct) =>
                {
                    var response = await client.PullAppInfoAsync(new PullAppInfoRequest
                    {
                        AppInfoId = new TuiHub.Protos.Librarian.V1.AppInfoID
                        {
                            Internal = false,
                            Source = Platform,
                            SourceAppId = appIdMq.AppId
                        }
                    }, cancellationToken: ct);
                    _logger.LogDebug($"Got response: {response}");
                    appInfoResp = response.AppInfo;
                }, cancellationToken: ct);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Cancelling worker");
                return;
            }
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            if (appIdMq.UpdateInternalAppInfoName)
            {
                var appInfo = db.AppInfos
                    .Where(x => x.IsInternal == false && x.Source == Platform && x.SourceAppId == appIdMq.AppId)
                    .Include(x => x.AppInfoDetails)
                    .Include(x => x.ParentAppInfo)
                    .Single(x => x.IsInternal == false && x.Source == Platform && x.SourceAppId == appIdMq.AppId);
                appInfo.ParentAppInfo!.Name = appInfoResp!.Name;
                appInfo.UpdateFromProtoAppInfo(appInfoResp);
            }
            else
            {
                var appInfo = db.AppInfos
                    .Where(x => x.IsInternal == false && x.Source == Platform && x.SourceAppId == appIdMq.AppId)
                    .Include(x => x.AppInfoDetails)
                    .Single(x => x.IsInternal == false && x.Source == Platform && x.SourceAppId == appIdMq.AppId);
                appInfo.UpdateFromProtoAppInfo(appInfoResp!);
            }
            await db.SaveChangesAsync(_cts.Token);
        }
    }
}
