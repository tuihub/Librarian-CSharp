using Librarian.Angela.Interfaces;
using Librarian.Angela.Providers;
using Librarian.Common.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Services
{
    public class PullMetadataService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly CancellationTokenSource _cts = new();
        private readonly ManualResetEventSlim _mres = new(false);

        private readonly Queue<long> _internalIDs = new();

        public PullMetadataService(ILogger<PullMetadataService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            Task.Run(() => MainPullMetadataThread(_cts.Token));
        }

        ~PullMetadataService()
        {
            _cts.Cancel();
        }

        private async void MainPullMetadataThread(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    _mres.Wait(token);
                    while (_internalIDs.Count > 0)
                    {
                        var internalID = _internalIDs.Dequeue();
                        _logger.LogDebug("Pulling app {Id}, _internalIDs.Count = {Count}", internalID, _internalIDs.Count);
                        AppSource appSource;
                        using (var scpoe = _serviceProvider.CreateScope())
                        {
                            using (var dbContext = scpoe.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                appSource = dbContext.Apps.Single(x => x.Id == internalID).Source;
                            }
                        }
                        var retries = 0;
                        while (retries < GlobalContext.SystemConfig.MetadataServiceMaxRetries)
                        {
                            try
                            {
                                if (appSource == AppSource.Steam)
                                {
                                    using var scope = _serviceProvider.CreateScope();
                                    var steamProvider = scope.ServiceProvider.GetRequiredService<ISteamProvider>();
                                    await steamProvider.PullAppAsync(internalID);
                                    break;
                                }
                                else if (appSource == AppSource.Vndb)
                                {
                                    using var scope = _serviceProvider.CreateScope();
                                    var vndbProvider = scope.ServiceProvider.GetRequiredService<IVndbProvider>();
                                    await vndbProvider.PullAppAsync(internalID);
                                    break;
                                }
                                else if (appSource == AppSource.Bangumi)
                                {
                                    using var scope = _serviceProvider.CreateScope();
                                    var bangumiProvider = scope.ServiceProvider.GetRequiredService<IBangumiProvider>();
                                    await bangumiProvider.PullAppAsync(internalID);
                                    break;
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }
                            }
                            catch (Exception ex)
                            {
                                retries++;
                                _logger.LogWarning(ex, "Failed to pull app {Id}, retries = {retries}, retrying in {RetrySeconds} seconds",
                                    internalID, retries, GlobalContext.SystemConfig.MetadataServiceRetrySeconds);
                                await Task.Delay(Convert.ToInt32(GlobalContext.SystemConfig.MetadataServiceRetrySeconds * 1000), token);
                            }
                        }

                        if (token.IsCancellationRequested)
                            throw new OperationCanceledException(token);
                        var pullIntervalSeconds = appSource switch
                        {
                            AppSource.Steam => GlobalContext.SystemConfig.PullSteamIntervalSeconds,
                            AppSource.Vndb => GlobalContext.SystemConfig.PullVndbIntervalSeconds,
                            AppSource.Bangumi => GlobalContext.SystemConfig.PullBangumiIntervalSeconds,
                            _ => throw new NotImplementedException()
                        };
                        _logger.LogDebug("Waiting for {IntervalSeconds} seconds, _internalIDs.Count = {Count}", pullIntervalSeconds, _internalIDs.Count);
                        await Task.Delay(Convert.ToInt32(pullIntervalSeconds * 1000), token);
                    }
                    _mres.Reset();
                }
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug(ex, "OperationCanceledException caught, exiting");
            }
        }

        public void AddPullApp(long internalID)
        {
            _internalIDs.Enqueue(internalID);
            _mres.Set();
        }
    }
}
