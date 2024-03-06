using Librarian.Angela.Interfaces;
using Librarian.Common.Utils;
using Librarian.ThirdParty.Bangumi;
using Librarian.ThirdParty.Steam;
using Librarian.ThirdParty.Vndb;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Providers
{
    public class BangumiProvider : IBangumiProvider
    {
        private readonly BangumiAPIService _bangumiAPIService;
        private readonly ApplicationDbContext _dbContext;

        private readonly string _bangumiAPIKey = GlobalContext.SystemConfig.BangumiAPIKey;

        public BangumiProvider(ApplicationDbContext dbContext)
        {
            _bangumiAPIService = new BangumiAPIService(_bangumiAPIKey);
            _dbContext = dbContext;
        }

        public async Task PullAppInfoAsync(long internalID)
        {
            var appInfo = _dbContext.AppInfos
                          .Where(x => x.Id == internalID)
                          .Include(x => x.AppInfoDetails)
                          .Single(x => x.Id == internalID);
            if (appInfo.Source != "bangumi")
            {
                throw new NotImplementedException();
            }
            else
            {
                var bangumiAppInfo = await _bangumiAPIService.GetAppInfoAsync(Convert.ToInt32(appInfo.SourceAppId));
                bangumiAppInfo.AppInfoDetails!.Description = "<div style=\"white-space: pre-line\">" +
                                                     bangumiAppInfo.AppInfoDetails!.Description +
                                                     "</div>";
                appInfo.UpdateFromAppInfo(bangumiAppInfo);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
