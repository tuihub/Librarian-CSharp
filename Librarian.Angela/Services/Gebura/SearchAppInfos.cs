using Grpc.Core;
using Librarian.Common.Helpers;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1.Angela;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Services
{
    public partial class AngelaService
    {
        [Authorize]
        public override async Task<SearchAppInfosResponse> SearchAppInfos(SearchAppInfosRequest request, ServerCallContext context)
        {
            // Verify that the user is an administrator
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

            // Get request parameters
            var appInfos = _dbContext.AppInfos.AsQueryable();

            // Fuzzy search by name
            if (!string.IsNullOrEmpty(request.NameLike))
            {
                appInfos = appInfos.Where(a => a.Name.Contains(request.NameLike));
            }

            // Filter by source
            if (request.SourceFilter.Count > 0)
            {
                appInfos = appInfos.Where(a => request.SourceFilter.Contains(a.Source.ToString()));
            }

            // Apply paging
            appInfos = appInfos.ApplyPagingRequest(request.Paging);

            // Get results
            var appInfosList = await appInfos.ToListAsync();
            var count = await appInfos.CountAsync();

            // Build response
            var response = new SearchAppInfosResponse
            {
                Paging = new PagingResponse { TotalSize = count }
            };

            // Get AppInfo objects and add to response
            foreach (var appInfo in appInfosList)
            {
                response.AppInfos.Add(appInfo.ToPB());
            }

            return response;
        }
    }
}