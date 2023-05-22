using Grpc.Core;
using Librarian.Sephirah.Models;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // TODO: only impl AssignedAppIdFilter
        [Authorize]
        public override Task<ListAppPackagesResponse> ListAppPackages(ListAppPackagesRequest request, ServerCallContext context)
        {
            var assignedAppIdFilter = request.AssignedAppIdFilter.Select(x => x.Id);
            var appPackages = _dbContext.AppPackages.Where(x => assignedAppIdFilter.Contains(x.AppId))
                                                    .ApplyPagingRequest(request.Paging);
            var ret = new ListAppPackagesResponse
            {
                Paging = new PagingResponse { TotalSize = appPackages.Count() }
            };
            ret.AppPackages.Add(appPackages.Select(x => x.ToProtoAppPackage()));
            return Task.FromResult(ret);
        }
    }
}
