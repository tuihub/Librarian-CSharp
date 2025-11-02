using Grpc.Core;
using Librarian.Common.Helpers;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    [Authorize]
    public override Task<ListAppsResponse> ListApps(ListAppsRequest request, ServerCallContext context)
    {
        var userId = context.GetInternalIdFromHeader();
        var ownerIdFilter = request.OwnerIdFilter;
        var idFilter = request.IdFilter;
        var apps = _dbContext.Apps.AsQueryable();
        if (idFilter.Count > 0) apps = apps.Where(x => idFilter.Select(y => y.Id).Contains(x.Id));
        if (ownerIdFilter.Count == 0)
        {
            apps = apps.Where(x => x.UserId == userId);
        }
        else
        {
            var ownerIds = ownerIdFilter.Select(y => y.Id).ToList();
            var includesSelf = ownerIds.Contains(userId);
            if (includesSelf)
            {
                var otherOwnerIds = ownerIds.Where(id => id != userId).ToList();
                apps = apps.Where(x => x.UserId == userId || (otherOwnerIds.Contains(x.UserId) && x.IsPublic == true));
            }
            else
            {
                apps = apps.Where(x => ownerIds.Contains(x.UserId) && x.IsPublic == true);
            }
        }

        apps = apps.OrderByDescending(x => x.UpdatedAt).ThenByDescending(x => x.Id);
        var totalSize = apps.Count();
        apps = apps.ApplyPagingRequest(request.Paging);
        var response = new ListAppsResponse
        {
            Paging = new PagingResponse { TotalSize = totalSize }
        };
        response.Apps.Add(apps.Select(x => x.ToPb()));
        return Task.FromResult(response);
    }
}