using Grpc.Core;
using Librarian.Common.Models.Db;
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
        public override Task<UpdateAppCategoryResponse> UpdateAppCategory(UpdateAppCategoryRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var appCategoryReq = request.AppCategory;
            var appCategory = _dbContext.AppCategories
                .Where(x => x.Id == appCategoryReq.Id.Id)
                .Include(x => x.User)
                .Include(x => x.AppInfos)
                .Include(x => x.Apps)
                .SingleOrDefault(x => x.Id == request.AppCategory.Id.Id);
            if (appCategory == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppCategory not exists."));
            }
            if (appCategory.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "AppCategory is not owned by user."));
            }
            appCategory.Name = request.AppCategory.Name;
            // update Apps associated with this AppCategory
            var appIds = appCategory.Apps.Select(x => x.Id);
            var appIdsReq = appCategoryReq.AppIds.Select(x => x.Id);
            var appIdsToAdd = appIdsReq.Where(x => appIds.Contains(x) == false);
            var appIdsToRemove = appIds.Where(x => appIdsReq.Contains(x) == false);
            appIdsToAdd.ToList().ForEach(x => appCategory.Apps.Add(new Common.Models.Db.App { Id = x }));
            appIdsToRemove.ToList().ForEach(x => appCategory.Apps.Remove(new Common.Models.Db.App { Id = x }));
            // update AppInfos associated with this AppCategory
            var appInfoIds = appCategory.AppInfos.Select(x => x.Id);
            var appInfoIdsReq = appCategoryReq.AppInfoIds.Select(x => x.Id);
            var appInfoIdsToAdd = appInfoIdsReq.Where(x => appInfoIds.Contains(x) == false);
            var appInfoIdsToRemove = appInfoIds.Where(x => appInfoIdsReq.Contains(x) == false);
            appInfoIdsToAdd.ToList().ForEach(x => appCategory.AppInfos.Add(new Common.Models.Db.AppInfo { Id = x }));
            appInfoIdsToRemove.ToList().ForEach(x => appCategory.AppInfos.Remove(new Common.Models.Db.AppInfo { Id = x }));
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppCategoryResponse());
        }
    }
}
