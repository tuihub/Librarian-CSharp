using Librarian.Angela.Interfaces;
using Librarian.Angela.Providers;
using Librarian.Common.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela
{
    public class PullMetadataService
    {
        private readonly ILogger _logger;
        private readonly ISteamProvider _steamProvider;
        private readonly CancellationTokenSource _cts = new();
        private readonly ManualResetEventSlim _steamProviderEvent = new(false);

        private readonly Queue<AppID> _appIDs = new();

        public PullMetadataService(ILogger<PullMetadataService> logger)
        {
            _logger = logger;
            _steamProvider = new SteamProvider(GlobalContext.SystemConfig.SteamAPIKey);

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
                    _steamProviderEvent.Wait(token);
                    while (_appIDs.Count > 0)
                    {
                        var appID = _appIDs.Dequeue();
                        try
                        {
                            await _steamProvider.PullAppAsync(appID);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to pull app {SourceAppId}, retrying in {RetrySeconds} seconds",
                                appID.SourceAppId, GlobalContext.SystemConfig.MetadataServiceRetrySeconds);
                            await Task.Delay(Convert.ToInt32(GlobalContext.SystemConfig.MetadataServiceRetrySeconds * 1000), token);
                        }

                        if (token.IsCancellationRequested)
                            throw new OperationCanceledException(token);
                        _logger.LogDebug("Waiting for {IntervalSeconds}, _appIDs.Count = {Count}",
                            GlobalContext.SystemConfig.PullSteamIntervalSeconds, _appIDs.Count);
                        await Task.Delay(Convert.ToInt32(GlobalContext.SystemConfig.PullSteamIntervalSeconds * 1000), token);
                    }
                    _steamProviderEvent.Reset();
                }
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogDebug(ex, "OperationCanceledException caught, exiting");
            }
        }

        public void AddPullApp(AppID appID)
        {
            _appIDs.Enqueue(appID);
            _steamProviderEvent.Set();
        }
    }
}
