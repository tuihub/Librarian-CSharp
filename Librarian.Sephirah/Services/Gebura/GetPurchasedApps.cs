using Grpc.Core;
using Librarian.Sephirah.Models;
using Librarian.Sephirah.Utils;
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
            var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
            var userId = JwtUtil.GetInternalIdFromToken(token);
            var apps = _dbContext.Users.Include(x => x.Apps)
                               .Single(x => x.Id == userId)
                               .Apps
                               .Select(x => x.GetAppWithoutDetails().ToProtoApp());
            var ret = new GetPurchasedAppsResponse();
            ret.Apps.Add(apps);
            return Task.FromResult(ret);
        }
    }
}
