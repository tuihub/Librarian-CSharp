using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>
        /// Apps without details
        /// </returns>
        [Authorize]
        public override Task<GetPurchasedAppsResponse> GetPurchasedApps(GetPurchasedAppsRequest request, ServerCallContext context)
        {
            var userId = JwtUtil.GetInternalIdFromJwt(context);
            var apps = _dbContext.Users.Include(x => x.Apps)
                               .Single(x => x.Id == userId)
                               .Apps
                               .Select(x => x.GetAppWithoutDetails().ToProtoApp());
            // add user-app-appCategoryIds
            var appAppCategories = _dbContext.UserAppAppCategories.Where(x => x.UserId == userId);
            foreach (var app in apps)
                app.AppCategoryIds.Add(appAppCategories.Where(x => x.AppId == app.Id.Id)
                                                       .Select(x => new InternalID { Id = x.AppCategoryId }));
            // construct return value
            var ret = new GetPurchasedAppsResponse();
            ret.Apps.Add(apps);
            return Task.FromResult(ret);
        }
    }
}
