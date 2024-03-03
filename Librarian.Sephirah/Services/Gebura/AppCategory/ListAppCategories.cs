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
        public override Task<ListAppCategoriesResponse> ListAppCategories(ListAppCategoriesRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var appCategories = _dbContext.AppCategories.Where(x => x.UserId == userId);
            var ret = new ListAppCategoriesResponse();
            ret.AppCategories.Add(appCategories.Select(x => x.ToProtoAppCategory()));
            // add user-app-appCategoryIds
            var userAppCategories = _dbContext.UserAppAppCategories.Where(x => x.UserId == userId);
            foreach (var appCategory in ret.AppCategories)
                appCategory.AppIds.Add(userAppCategories.Where(x => x.AppCategoryId == appCategory.Id.Id)
                                                       .Select(x => new InternalID { Id = x.AppId }));
            return Task.FromResult(ret);
        }
    }
}
