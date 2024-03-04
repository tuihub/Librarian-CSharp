using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<ListAppInfosResponse> ListAppInfos(ListAppInfosRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            // get request param
            var excludeInternal = request.ExcludeInternal;
            var sourceFilters = request.SourceFilter;
            var typeFilters = request.TypeFilter;
            var idFilters = request.IdFilter;
            var containDetails = request.ContainDetails;
            // filter appInfos
            IQueryable<Common.Models.AppInfo> appInfos;
            if (containDetails == false) { appInfos = _dbContext.AppInfos.AsQueryable(); }
            else { appInfos = _dbContext.AppInfos.Include(x => x.AppInfoDetails).AsQueryable(); }
            if (excludeInternal == true)
            {
                appInfos = appInfos.Where(x => x.IsInternal == false);
            }
            if (idFilters.Count > 0)
            {
                appInfos = appInfos.Where(x => idFilters.Select(x => x.Id).Contains(x.Id));
            }
            if (typeFilters.Count > 0)
            {
                appInfos = appInfos.Where(x => typeFilters.Contains(x.Type));
            }
            if (sourceFilters.Count > 0)
            {
                appInfos = appInfos.Where(x => sourceFilters.Contains(x.Source));
            }
            appInfos = appInfos.ApplyPagingRequest(request.Paging);
            if (containDetails == false)
            {
                appInfos = appInfos.Select(x => x.GetAppInfoWithoutDetails());
            }
            // construct response
            var response = new ListAppInfosResponse
            {
                Paging = new TuiHub.Protos.Librarian.V1.PagingResponse { TotalSize = appInfos.Count() }
            };
            response.AppInfos.Add(appInfos.Select(x => x.ToProtoAppInfo()));
            return Task.FromResult(response);
        }
    }
}
