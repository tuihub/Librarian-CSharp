using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<ListAppPackagesResponse> ListAppPackages(ListAppPackagesRequest request, ServerCallContext context)
        {
            var sourceFilter = request.SourceFilter;
            var idFilter = request.IdFilter;
            var assignedAppIdFilter = request.AssignedAppIdFilter;
            var appPackages = _dbContext.AppPackages.AsQueryable();
            if (idFilter.Count > 0)
                appPackages = appPackages.Where(x => idFilter.Select(x => x.Id).Contains(x.Id));
            if (assignedAppIdFilter.Count > 0)
                appPackages = appPackages.Where(x => assignedAppIdFilter.Select(x => x.Id).Contains(x.Id));
            if (sourceFilter.Count > 0)
                appPackages = appPackages.Where(x => sourceFilter.Contains(x.Source));
            appPackages = appPackages.ApplyPagingRequest(request.Paging);
            var ret = new ListAppPackagesResponse
            {
                Paging = new PagingResponse { TotalSize = appPackages.Count() }
            };
            ret.AppPackages.Add(appPackages.Select(x => x.ToProtoAppPackage()));
            return Task.FromResult(ret);
        }
    }
}
