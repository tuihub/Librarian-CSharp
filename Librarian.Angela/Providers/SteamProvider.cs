using Librarian.Angela.Interfaces;
using Librarian.Common.Utils;
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
        private readonly ApplicationDbContext _dbContext;

        public SteamProvider(string steamAPIKey, ApplicationDbContext dbContext)
        {
            _steamAPIKey = steamAPIKey;
            _steamAPIService = new SteamAPIService(_steamAPIKey);
            _dbContext = dbContext;
        }

        public async Task PullAppAsync(long internalID)
        {
            var app = _dbContext.Apps.Single(x => x.Id == internalID);
            if (app.Source != TuiHub.Protos.Librarian.V1.AppSource.Steam)
            {
                throw new NotImplementedException();
            }
            else
            {
                app.UpdateFromApp(await _steamAPIService.GetAppAsync(Convert.ToUInt32(app.SourceAppId)));
            }
        }
    }
}
