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
using static System.Formats.Asn1.AsnWriter;

namespace Librarian.Angela.Services
{
    public class PullMetadataService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        private readonly CancellationTokenSource _cts = new();
        private readonly ManualResetEventSlim _mres = new(false);

        private readonly Queue<ExternalApp> _externalApps = new();

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
                    while (_externalApps.Count > 0)
                    {
                        var externalApp = _externalApps.Dequeue();
                        _logger.LogDebug("Pulling app {Id}, UpdateParentAppName = {UpdateParentAppName}," +
                            " _externalApps.Count = {Count}", 
                            externalApp.InternalID, externalApp.UpdateParentAppName.ToString(), _externalApps.Count);
                        string appSource;
                        long? parentAppId;
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            using (var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                var app = dbContext.AppInfos.Single(x => x.Id == externalApp.InternalID);
                                appSource = app.Source;
                                parentAppId = app.ParentAppInfoId;
                            }
                        }
                        var retries = 0;
                        while (retries < GlobalContext.SystemConfig.MetadataServiceMaxRetries)
                        {
                            try
                            {
                                if (appSource == "steam")
                                {
                                    using var scope = _serviceProvider.CreateScope();
                                    var steamProvider = scope.ServiceProvider.GetRequiredService<ISteamProvider>();
                                    await steamProvider.PullAppInfoAsync(externalApp.InternalID);
                                }
                                else if (appSource == "vndb")
                                {
                                    using var scope = _serviceProvider.CreateScope();
                                    var vndbProvider = scope.ServiceProvider.GetRequiredService<IVndbProvider>();
                                    await vndbProvider.PullAppInfoAsync(externalApp.InternalID);
                                }
                                else if (appSource == "bangumi")
                                {
                                    using var scope = _serviceProvider.CreateScope();
                                    var bangumiProvider = scope.ServiceProvider.GetRequiredService<IBangumiProvider>();
                                    await bangumiProvider.PullAppInfoAsync(externalApp.InternalID);
                                }
                                else
                                {
                                    _logger.LogWarning("Unsupported app Id = {Id}, appSource {AppSource}", externalApp.InternalID, appSource);
                                    break;
                                }

                                // update parent app name
                                if (externalApp.UpdateParentAppName == true)
                                {
                                    if (parentAppId == null)
                                    {
                                        _logger.LogWarning("Parent app Id is null, skipping");
                                    }
                                    else
                                    {
                                        _logger.LogDebug("Updating internal app(Id = {Id}) using external app(Id = {ExtId})", parentAppId, externalApp.InternalID);
                                        using (var scope = _serviceProvider.CreateScope())
                                        {
                                            using (var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                                            {
                                                var app = dbContext.AppInfos.Single(x => x.Id == parentAppId);
                                                app.Name = dbContext.AppInfos.Single(x => x.Id == externalApp.InternalID).Name;
                                                await dbContext.SaveChangesAsync();
                                            }
                                        }
                                    }
                                }

                                // break out of retry loop
                                break;
                            }
                            catch (Exception ex)
                            {
                                retries++;
                                _logger.LogWarning(ex, "Failed to pull app {Id}, retries = {retries}, retrying in {RetrySeconds} seconds",
                                    externalApp.InternalID, retries, GlobalContext.SystemConfig.MetadataServiceRetrySeconds);
                                await Task.Delay(Convert.ToInt32(GlobalContext.SystemConfig.MetadataServiceRetrySeconds * 1000), token);
                            }
                        }

                        if (token.IsCancellationRequested)
                            throw new OperationCanceledException(token);
                        var pullIntervalSeconds = appSource switch
                        {
                            "steam" => GlobalContext.SystemConfig.PullSteamIntervalSeconds,
                            "vndb" => GlobalContext.SystemConfig.PullVndbIntervalSeconds,
                            "bangumi" => GlobalContext.SystemConfig.PullBangumiIntervalSeconds,
                            _ => throw new NotImplementedException()
                        };
                        _logger.LogDebug("Waiting for {IntervalSeconds} seconds, _externalApps.Count = {Count}", pullIntervalSeconds, _externalApps.Count);
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

        public void AddPullApp(long internalID, bool updateParentAppName = false)
        {
            _externalApps.Enqueue(new ExternalApp
            {
                InternalID = internalID,
                UpdateParentAppName = updateParentAppName
            });
            _mres.Set();
        }

        private class ExternalApp
        {
            public long InternalID { get; set; }
            public bool UpdateParentAppName { get; set; }
        }
    }
}
