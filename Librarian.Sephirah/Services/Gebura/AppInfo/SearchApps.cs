using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<SearchAppsResponse> SearchApps(SearchAppsRequest request, ServerCallContext context)
        {
            // get request param
            string keyword = request.Keywords;
            // filter apps
            var apps = _dbContext.AppInfos.AsQueryable();
            // TODO: update SearchApps
            apps = apps.Where(a => a.Source == Common.Constants.Proto.AppSourceInternal)
                       .Where(a => a.Name.Contains(keyword))
                       .Include(a => a.ChildApps);
            apps = apps.ApplyPagingRequest(request.Paging);
            // construct response
            var response = new SearchAppsResponse
            {
                Paging = new TuiHub.Protos.Librarian.V1.PagingResponse { TotalSize = apps.Count() }
            };
            response.Apps.Add(apps.Select(x => x.Flatten().ToProtoAppMixed()));
            return Task.FromResult(response);
        }
    }
}
