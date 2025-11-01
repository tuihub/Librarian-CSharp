using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using App = Librarian.Common.Models.Db.App;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    [Authorize]
    public override Task<UpdateAppCategoryResponse> UpdateAppCategory(UpdateAppCategoryRequest request,
        ServerCallContext context)
    {
        var userId = context.GetInternalIdFromHeader();
        var appCategoryReq = request.AppCategory;
        var appCategory = _dbContext.AppCategories
            .Where(x => x.Id == appCategoryReq.Id.Id)
            .Include(x => x.User)
            .Include(x => x.Apps)
            .SingleOrDefault(x => x.Id == request.AppCategory.Id.Id);
        if (appCategory == null)
            throw new RpcException(new Status(StatusCode.NotFound, "AppCategory not exists."));
        if (appCategory.UserId != userId)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "AppCategory is not owned by user."));
        appCategory.Name = request.AppCategory.Name;
        // update Apps associated with this AppCategory
        var appIds = appCategory.Apps.Select(x => x.Id);
        var appIdsReq = appCategoryReq.AppIds.Select(x => x.Id);
        var appIdsToAdd = appIdsReq.Where(x => !appIds.Contains(x));
        var appIdsToRemove = appIds.Where(x => !appIdsReq.Contains(x));
        appIdsToAdd.ToList().ForEach(x => appCategory.Apps.Add(new App { Id = x }));
        appIdsToRemove.ToList().ForEach(x => appCategory.Apps.Remove(new App { Id = x }));

        _dbContext.SaveChanges();
        return Task.FromResult(new UpdateAppCategoryResponse());
    }
}