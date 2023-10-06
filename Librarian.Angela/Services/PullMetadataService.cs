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

namespace Librarian.Angela.Services
{
    public class PullMetadataService
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;

        private readonly ISteamProvider _steamProvider;
        private readonly CancellationTokenSource _cts = new();
        private readonly ManualResetEventSlim _steamProviderEvent = new(false);

        private readonly Queue<long> _internalIDs = new();

        public PullMetadataService(ILogger<PullMetadataService> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            _steamProvider = new SteamProvider(GlobalContext.SystemConfig.SteamAPIKey, _dbContext);

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
                    while (_internalIDs.Count > 0)
                    {
                        var internalID = _internalIDs.Dequeue();
                        try
                        {
                            await _steamProvider.PullAppAsync(internalID);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to pull steam app {Id}, retrying in {RetrySeconds} seconds",
                                internalID, GlobalContext.SystemConfig.MetadataServiceRetrySeconds);
                            await Task.Delay(Convert.ToInt32(GlobalContext.SystemConfig.MetadataServiceRetrySeconds * 1000), token);
                        }

                        if (token.IsCancellationRequested)
                            throw new OperationCanceledException(token);
                        _logger.LogDebug("Waiting for {IntervalSeconds} seconds, _appIDs.Count = {Count}",
                            GlobalContext.SystemConfig.PullSteamIntervalSeconds, _internalIDs.Count);
                        await Task.Delay(Convert.ToInt32(GlobalContext.SystemConfig.PullSteamIntervalSeconds * 1000), token);
                    }
                    _steamProviderEvent.Reset();
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
            _steamProviderEvent.Set();
        }
    }
}
