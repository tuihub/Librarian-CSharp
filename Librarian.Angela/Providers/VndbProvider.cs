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

        public async Task PullAppAsync(long internalID)
        {
            var app = _dbContext.Apps
                      .Include(x => x.AppInfoDetails)
                      .Single(x => x.Id == internalID);
            if (app.Source != "vndb")
            {
                throw new NotImplementedException();
            }
            else
            {
                var vndbApp = await _vndbTcpAPIService.GetAppAsync(Convert.ToUInt32(app.SourceAppId));
                vndbApp.AppInfoDetails!.Description = "<div style=\"white-space: pre-line\">" +
                                                  vndbApp.AppInfoDetails!.Description +
                                                  "</div>";
                app.UpdateFromAppInfo(vndbApp);
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
