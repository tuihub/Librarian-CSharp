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

        private Queue<AppID> _appIDs = new();

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
                _steamProviderEvent.Wait(token);
                while (_appIDs.Count > 0)
                {
                    var appID = _appIDs.Dequeue();
                    await _steamProvider.PullAppAsync(appID);

                    if (token.IsCancellationRequested)
                        throw new OperationCanceledException(token);
                    Thread.Sleep(10000);
                }
            }
            catch(OperationCanceledException ex)
            {
                _logger.LogDebug(ex, "OperationCanceledException caught, exiting");
            }
        }
    }
}
