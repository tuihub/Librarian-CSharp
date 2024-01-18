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
        public override Task<ListAppsResponse> ListApps(ListAppsRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            // get request param
            var sourceFilters = request.SourceFilter;
            var typeFilters = request.TypeFilter;
            var idFilters = request.IdFilter;
            var containDetails = request.ContainDetails;
            // filter apps
            IQueryable<Common.Models.App> apps;
            if (containDetails == false) { apps = _dbContext.Apps.AsQueryable(); }
            else { apps = _dbContext.Apps.Include(x => x.AppDetails).AsQueryable(); }
            if (idFilters.Count > 0)
            {
                apps = apps.Where(x => idFilters.Select(x => x.Id).Contains(x.Id));
            }
            if (typeFilters.Count > 0)
            {
                apps = apps.Where(x => typeFilters.Contains(x.Type));
            }
            if (sourceFilters.Count > 0)
            {
                apps = apps.Where(x => sourceFilters.Contains(x.Source));
            }
            apps = apps.ApplyPagingRequest(request.Paging);
            if (containDetails == false)
            {
                apps = apps.Select(x => x.GetAppWithoutDetails());
            }
            // construct response
            var response = new ListAppsResponse
            {
                Paging = new TuiHub.Protos.Librarian.V1.PagingResponse { TotalSize = apps.Count() }
            };
            response.Apps.Add(apps.Select(x => x.ToProtoApp()));
            return Task.FromResult(response);
        }
    }
}
