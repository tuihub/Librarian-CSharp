using Grpc.Core;
using Librarian.Sephirah.Utils;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<ListAppsResponse> ListApps(ListAppsRequest request, ServerCallContext context)
        {
            using var db = new TestDbContext();
            // verify user type(admin)
            if (UserUtil.GetUserTypeFromToken(context, db) != UserType.Admin)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            // get request param
            var sourceFilters = request.SourceFilter;
            var typeFilters = request.TypeFilter;
            var idFilters = request.IdFilter;
            var containDetails = request.ContainDetails;
            // filter apps
            IEnumerable<Models.App> apps = db.Apps;
            if (idFilters.Count > 0)
                apps = apps.Where(x => idFilters.Select(x => x.Id).Contains(x.InternalId));
            if (typeFilters.Count > 0)
                apps = apps.Where(x => typeFilters.Contains(x.Type));
            if (sourceFilters.Count > 0)
                apps = apps.Where(x => sourceFilters.Contains(x.Source));
            apps = apps.ApplyPagingRequest(request.Paging);
            if (containDetails == false)
                apps = apps.Select(x => x.GetAppWithoutDetails());
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
