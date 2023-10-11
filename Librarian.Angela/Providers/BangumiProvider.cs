﻿using Librarian.Angela.Interfaces;
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

        public async Task PullAppAsync(long internalID)
        {
            var app = _dbContext.Apps
                      .Include(x => x.AppDetails)
                      .Single(x => x.Id == internalID);
            if (app.Source != TuiHub.Protos.Librarian.V1.AppSource.Bangumi)
            {
                throw new NotImplementedException();
            }
            else
            {
                app.UpdateFromApp(await _bangumiAPIService.GetAppAsync(Convert.ToInt32(app.SourceAppId)));
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}