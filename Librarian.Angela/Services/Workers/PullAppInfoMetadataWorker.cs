using Grpc.Core;
using Grpc.Net.Client;
using Librarian.Common;
using Librarian.Common.Contracts;
using Librarian.Common.Converters;
using Librarian.Common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using TuiHub.Protos.Librarian.Porter.V1;

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
            var workerFunc = new Func<object, CancellationToken, Task>(WorkerFunc);
            _messageQueueService.SubscribeQueue(_channel, Platform, workerFunc, typeof(AppIdMQ), ct);
        }

        // TODO: remove appIdMq.UpdateInternalAppInfoName, impl AppInfoDetails, impl UpdateFromProto (Porter Model)
        private async Task WorkerFunc(object appIdMqObj, CancellationToken ct = default)
        {
            if (appIdMqObj is not AppIdMQ)
            {
                _logger.LogWarning("Cannot convert {appIdMqObj} to AppIdMQ", appIdMqObj);
                return;
            }
            var appIdMq = (AppIdMQ)appIdMqObj;
            _logger.LogDebug($"Worker ({ServiceId}, {Platform}) got message: {appIdMq}");
            var client = new LibrarianPorterService.LibrarianPorterServiceClient(_grpcChannel);
            GetAppInfoResponse? response = null;
            try
            {
                await _retryPolicy.ExecuteAsync(async (ct) =>
                {
                    response = await client.GetAppInfoAsync(new GetAppInfoRequest
                    {
                        Source = Platform,
                        SourceAppId = appIdMq.AppId
                    }, cancellationToken: ct);
                    _logger.LogDebug($"Got response: {response}");
                }, cancellationToken: ct);
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Cancelling worker");
                return;
            }
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var appInfo = db.AppInfos
                .Where(x => x.Source.ToString() == Platform && x.SourceAppId == appIdMq.AppId)
                //.Include(x => x.AppInfoDetails)
                .Single(x => x.Source.ToString() == Platform && x.SourceAppId == appIdMq.AppId);
            //appInfo.UpdateFromProto(response!.AppInfo);
            await db.SaveChangesAsync(_cts.Token);
        }
    }
}
