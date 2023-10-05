using Librarian.Angela.Interfaces;
using Librarian.ThirdParty.Steam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Providers
{
    public class SteamProvider : ISteamProvider
    {
        private readonly string _steamAPIKey;
        private readonly SteamAPIService _steamAPIService;

        public SteamProvider(string steamAPIKey)
        {
            _steamAPIKey = steamAPIKey;
            _steamAPIService = new SteamAPIService(_steamAPIKey);
        }

        public Task PullAppAsync(AppID appID)
        {
            throw new NotImplementedException();
        }
    }
}
