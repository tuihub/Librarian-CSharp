﻿using Grpc.Core;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<SearchAppsResponse> SearchApps(SearchAppsRequest request, ServerCallContext context)
        {
            using var db = new TestDbContext();
            // get request param
            string keyword = request.Keywords;
            // filter apps
            IEnumerable<Models.App> apps = db.Apps;
            apps = apps.Where(a => a.Name.Contains(keyword));
            apps = apps.ApplyPagingRequest(request.Paging);
            // construct response
            var response = new SearchAppsResponse
            {
                Paging = new TuiHub.Protos.Librarian.V1.PagingResponse { TotalSize = apps.Count() }
            };
            response.Apps.Add(apps.Select(x => x.ToProtoApp()));
            return Task.FromResult(response);
        }
    }
}
