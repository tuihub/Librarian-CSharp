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
        public override Task<ListAppInstsResponse> ListAppInsts(ListAppInstsRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var deviceIdFilter = request.DeviceIdFilter;
            var idFilter = request.IdFilter;
            var appIdFilter = request.AppIdFilter;
            IQueryable<Common.Models.AppInst> appInsts = _dbContext.AppInsts;
            if (idFilter.Count > 0)
            {
                appInsts = appInsts.Where(x => idFilter.Select(x => x.Id).Contains(x.Id));
            }
            if (deviceIdFilter.Count > 0)
            {
                appInsts = appInsts.Where(x => deviceIdFilter.Select(x => x.Id).Contains(x.DeviceId));
            }
            if (appIdFilter.Count > 0)
            {
                appInsts = appInsts.Where(x => appIdFilter.Select(x => x.Id).Contains(x.AppId));
            }
            appInsts = appInsts.ApplyPagingRequest(request.Paging);
            return Task.FromResult(new ListAppInstsResponse
            {
                Paging = new PagingResponse { TotalSize = appInsts.Count() },
                AppInsts = { appInsts.Select(x => x.ToProtoAppInst()) }
            });
        }
    }
}
