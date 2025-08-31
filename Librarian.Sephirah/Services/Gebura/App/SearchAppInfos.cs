using Grpc.Core;
using Librarian.Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<SearchAppInfosResponse> SearchAppInfos(SearchAppInfosRequest request, ServerCallContext context)
        {
            string nameLike = request.NameLike;
            var sourceFilter = request.SourceFilter;

            var appInfos = _dbContext.AppInfos.AsQueryable();

            if (!string.IsNullOrEmpty(nameLike))
            {
                appInfos = appInfos.Where(a => a.Name.Contains(nameLike));
            }
            if (sourceFilter.Count > 0)
            {
                appInfos = appInfos.Where(a => sourceFilter.Contains(a.Source.ToString()));
            }

            appInfos = appInfos.ApplyPagingRequest(request.Paging);

            var response = new SearchAppInfosResponse
            {
                Paging = new PagingResponse { TotalSize = appInfos.Count() }
            };
            response.AppInfos.Add(appInfos.Select(x => x.Flatten().ToPb()));
            return Task.FromResult(response);
        }
    }
}