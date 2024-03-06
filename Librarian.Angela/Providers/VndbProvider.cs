using Librarian.Angela.Interfaces;
using Librarian.Common.Utils;
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
    public class VndbProvider : IVndbProvider
    {
        private readonly VndbTcpAPIService _vndbTcpAPIService;
        private readonly ApplicationDbContext _dbContext;

        public VndbProvider(ApplicationDbContext dbContext)
        {
            _vndbTcpAPIService = new VndbTcpAPIService();
            _dbContext = dbContext;
        }

        public async Task PullAppInfoAsync(long internalID)
        {
            var appInfo = _dbContext.AppInfos
                          .Where(x => x.Id == internalID)
                          .Include(x => x.AppInfoDetails)
                          .Single(x => x.Id == internalID);
            if (appInfo.Source != "vndb")
            {
                throw new NotImplementedException();
            }
            else
            {
                var vndbAppInfo = await _vndbTcpAPIService.GetAppInfoAsync(Convert.ToUInt32(appInfo.SourceAppId));
                vndbAppInfo.AppInfoDetails!.Description = "<div style=\"white-space: pre-line\">" +
                                                  vndbAppInfo.AppInfoDetails!.Description +
                                                  "</div>";
                appInfo.UpdateFromAppInfo(vndbAppInfo);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
