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
            // get request param
            var pageNum = request.Paging.PageNum;
            var pageSize = request.Paging.PageSize;
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
            apps = apps.Skip(pageSize * (pageNum - 1)).Take(pageSize);
            if (containDetails == false)
                apps = apps.Select(x => x.GetAppWithoutDetails());
            // construct response
            var response = new ListAppsResponse
            {
                Paging = new TuiHub.Protos.Librarian.V1.PagingResponse { TotalSize = apps.Count() }
            };
            response.Apps.Add(apps.Select(x => new TuiHub.Protos.Librarian.V1.App
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = x.InternalId },
                Source = x.Source,
                SourceAppId = x.SourceAppId,
                SourceUrl = x.SourceUrl,
                Name = x.Name,
                Type = x.Type,
                ShortDescription = x.ShortDescription,
                ImageUrl = x.ImageUrl
            }));
            return Task.FromResult(response);
        }
    }
}
