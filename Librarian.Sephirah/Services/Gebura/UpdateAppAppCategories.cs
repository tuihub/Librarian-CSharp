using Grpc.Core;
using Librarian.Sephirah.Models;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<UpdateAppAppCategoriesResponse> UpdateAppAppCategories(UpdateAppAppCategoriesRequest request, ServerCallContext context)
        {
            var userId = JwtUtil.GetInternalIdFromJwt(context);
            var appId = request.AppId.Id;
            var appCategoryIds = request.AppCategoryIds.Select(x => x.Id);
            var userAppAppCategories = _dbContext.UserAppAppCategories
                                                 .Where(x => x.UserId == userId)
                                                 .Where(x => x.AppId == appId);
            foreach (var appCategoryId in appCategoryIds)
                if (userAppAppCategories.Count(x => x.AppCategoryId == appCategoryId) == 0)
                    _dbContext.UserAppAppCategories.Add(new UserAppAppCategory
                    {
                        UserId = userId,
                        AppId = appId,
                        AppCategoryId = appCategoryId
                    });
            foreach (var userAppAppCategory in userAppAppCategories)
                if (appCategoryIds.Contains(userAppAppCategory.AppCategoryId) == false)
                    _dbContext.UserAppAppCategories.Remove(userAppAppCategory);
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppAppCategoriesResponse());
        }
    }
}
