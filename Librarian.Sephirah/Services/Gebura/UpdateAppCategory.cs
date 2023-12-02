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
        public override Task<UpdateAppCategoryResponse> UpdateAppCategory(UpdateAppCategoryRequest request, ServerCallContext context)
        {
            var userId = JwtUtil.GetInternalIdFromJwt(context);
            var appCategory = _dbContext.AppCategories.SingleOrDefault(x => x.Id == request.AppCategory.Id.Id);
            if (appCategory == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppCategory not exists."));
            if (appCategory.UserId != userId)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            appCategory.Name = request.AppCategory.Name;
            // update Apps associated with this AppCategory
            var appIds = request.AppCategory.AppIds.Select(x => x.Id);
            var userAppAppCategories = _dbContext.UserAppAppCategories
                                                 .Where(x => x.UserId == userId)
                                                 .Where(x => x.AppCategoryId == appCategory.Id);
            foreach (var appId in appIds)
                if (userAppAppCategories.Any(x => x.AppId == appId) == false)
                    _dbContext.UserAppAppCategories.Add(new UserAppAppCategory
                    {
                        UserId = userId,
                        AppId = appId,
                        AppCategoryId = appCategory.Id
                    });
            _dbContext.UserAppAppCategories.RemoveRange(userAppAppCategories.Where(x => appIds.Contains(x.AppId) == false));
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppCategoryResponse());
        }
    }
}
