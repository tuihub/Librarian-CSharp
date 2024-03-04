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

        private readonly Queue<ExternalAppInfo> _externalAppInfos = new();

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
                    while (_externalAppInfos.Count > 0)
                    {
                        var externalAppInfo = _externalAppInfos.Dequeue();
                        _logger.LogDebug("Pulling appInfo {Id}, UpdateParentAppInfoName = {UpdateParentAppInfoName}," +
                            " _externalAppInfos.Count = {Count}", 
                            externalAppInfo.InternalID, externalAppInfo.UpdateParentAppInfoName.ToString(), _externalAppInfos.Count);
                        string appInfoSource;
                        long? parentAppInfoId;
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            using (var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                var appInfo = dbContext.AppInfos.Single(x => x.Id == externalAppInfo.InternalID);
                                appInfoSource = appInfo.Source;
                                parentAppInfoId = appInfo.ParentAppInfoId;
                            }
                        }
                        var retries = 0;
                        while (retries < GlobalContext.SystemConfig.MetadataServiceMaxRetries)
                        {
                            try
                            {
                                if (appInfoSource == "steam")
                                {
                                    using var scope = _serviceProvider.CreateScope();
                                    var steamProvider = scope.ServiceProvider.GetRequiredService<ISteamProvider>();
                                    await steamProvider.PullAppInfoAsync(externalAppInfo.InternalID);
                                }
                                else if (appInfoSource == "vndb")
                                {
                                    using var scope = _serviceProvider.CreateScope();
                                    var vndbProvider = scope.ServiceProvider.GetRequiredService<IVndbProvider>();
                                    await vndbProvider.PullAppInfoAsync(externalAppInfo.InternalID);
                                }
                                else if (appInfoSource == "bangumi")
                                {
                                    using var scope = _serviceProvider.CreateScope();
                                    var bangumiProvider = scope.ServiceProvider.GetRequiredService<IBangumiProvider>();
                                    await bangumiProvider.PullAppInfoAsync(externalAppInfo.InternalID);
                                }
                                else
                                {
                                    _logger.LogWarning("Unsupported appInfo Id = {Id}, appInfoSource {AppInfoSource}", externalAppInfo.InternalID, appInfoSource);
                                    break;
                                }

                                // update parent app name
                                if (externalAppInfo.UpdateParentAppInfoName == true)
                                {
                                    if (parentAppInfoId == null)
                                    {
                                        _logger.LogWarning("Parent app Id is null, skipping");
                                    }
                                    else
                                    {
                                        _logger.LogDebug("Updating internal appInfo(Id = {Id}) using external appInfo(Id = {ExtId})", parentAppInfoId, externalAppInfo.InternalID);
                                        using (var scope = _serviceProvider.CreateScope())
                                        {
                                            using (var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
                                            {
                                                var appInfo = dbContext.AppInfos.Single(x => x.Id == parentAppInfoId);
                                                appInfo.Name = dbContext.AppInfos.Single(x => x.Id == externalAppInfo.InternalID).Name;
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
                                _logger.LogWarning(ex, "Failed to pull appInfo {Id}, retries = {retries}, retrying in {RetrySeconds} seconds",
                                    externalAppInfo.InternalID, retries, GlobalContext.SystemConfig.MetadataServiceRetrySeconds);
                                await Task.Delay(Convert.ToInt32(GlobalContext.SystemConfig.MetadataServiceRetrySeconds * 1000), token);
                            }
                        }

                        if (token.IsCancellationRequested)
                            throw new OperationCanceledException(token);
                        var pullIntervalSeconds = appInfoSource switch
                        {
                            "steam" => GlobalContext.SystemConfig.PullSteamIntervalSeconds,
                            "vndb" => GlobalContext.SystemConfig.PullVndbIntervalSeconds,
                            "bangumi" => GlobalContext.SystemConfig.PullBangumiIntervalSeconds,
                            _ => throw new NotImplementedException()
                        };
                        _logger.LogDebug("Waiting for {IntervalSeconds} seconds, _externalAppInfos.Count = {Count}", pullIntervalSeconds, _externalAppInfos.Count);
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

        public void AddPullAppInfo(long internalID, bool updateParentAppName = false)
        {
            _externalAppInfos.Enqueue(new ExternalAppInfo
            {
                InternalID = internalID,
                UpdateParentAppInfoName = updateParentAppName
            });
            _mres.Set();
        }

        private class ExternalAppInfo
        {
            public long InternalID { get; set; }
            public bool UpdateParentAppInfoName { get; set; }
        }
    }
}
