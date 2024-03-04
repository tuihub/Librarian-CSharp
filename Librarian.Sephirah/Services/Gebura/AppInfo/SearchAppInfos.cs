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
        public override Task<SearchAppInfosResponse> SearchAppInfos(SearchAppInfosRequest request, ServerCallContext context)
        {
            // get request param
            string query = request.Query;
            // filter apps
            var appInfos = _dbContext.AppInfos.AsQueryable();
            // TODO: update SearchAppInfos
            appInfos = appInfos.Where(a => a.Source == Common.Constants.Proto.AppInfoSourceInternal)
                       .Where(a => a.Name.Contains(query))
                       .Include(a => a.ChildAppInfos);
            appInfos = appInfos.ApplyPagingRequest(request.Paging);
            // construct response
            var response = new SearchAppInfosResponse
            {
                Paging = new TuiHub.Protos.Librarian.V1.PagingResponse { TotalSize = appInfos.Count() }
            };
            response.AppInfos.Add(appInfos.Select(x => x.Flatten().ToProtoAppInfoMixed()));
            return Task.FromResult(response);
        }
    }
}
