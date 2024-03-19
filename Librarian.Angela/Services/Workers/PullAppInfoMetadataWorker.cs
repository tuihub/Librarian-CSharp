using Consul;
using Grpc.Core;
using Grpc.Net.Client;
using Librarian.Common.Contracts;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly Thread _worker;
        private readonly AsyncRetryPolicy _retryPolicy;

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
            _worker = new Thread(() => Worker(_cts.Token));
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
        }

        public void Start()
        {
            _worker.Start();
        }

        public void Cancel()
        {
            _cts.Cancel();
        }

        private void Worker(CancellationToken ct)
        {
            _logger.LogInformation($"Worker for ({ServiceId}, {Platform}) started");
            _messageQueueService.SubscribeQueue($"{Platform}", async appIdMq =>
            {
                _logger.LogDebug($"Worker ({ServiceId}, {Platform}) got message: {appIdMq}");
                var client = new LibrarianPorterService.LibrarianPorterServiceClient(_grpcChannel);
                TuiHub.Protos.Librarian.V1.AppInfo? appInfoResp = null;
                await _retryPolicy.ExecuteAsync(async () =>
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
                });
                if (ct.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancelling subscription");
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
                await db.SaveChangesAsync();
            }, ct);
        }
    }
}
