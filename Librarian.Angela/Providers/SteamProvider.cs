using Librarian.Angela.Interfaces;
using Librarian.Common.Utils;
using Librarian.ThirdParty.Steam;
using Microsoft.EntityFrameworkCore;
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
        private readonly SteamAPIService _steamAPIService;
        private readonly ApplicationDbContext _dbContext;

        private readonly string _steamAPIKey = GlobalContext.SystemConfig.SteamAPIKey;

        public SteamProvider(ApplicationDbContext dbContext)
        {
            _steamAPIService = new SteamAPIService(_steamAPIKey);
            _dbContext = dbContext;
        }

        public async Task PullAppInfoAsync(long internalID)
        {
            var appInfo = _dbContext.AppInfos
                          .Include(x => x.AppInfoDetails)
                          .Single(x => x.Id == internalID);
            if (appInfo.Source != "steam")
            {
                throw new NotImplementedException();
            }
            else
            {
                appInfo.UpdateFromAppInfo(await _steamAPIService.GetAppInfoAsync(Convert.ToUInt32(appInfo.SourceAppId), "cn", "schinese"));
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
