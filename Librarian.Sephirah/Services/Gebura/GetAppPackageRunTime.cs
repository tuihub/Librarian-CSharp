using Google.Protobuf.WellKnownTypes;
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
        public override Task<GetAppPackageRunTimeResponse> GetAppPackageRunTime(GetAppPackageRunTimeRequest request, ServerCallContext context)
        {
            var userId = JwtUtil.GetInternalIdFromJwt(context);
            var appPackageId = request.AppPackageId.Id;
            var totalRunTime = _dbContext.UserAppPackages
                                         .Single(x => x.UserId == userId &&
                                                      x.AppPackageId == appPackageId)
                                         .TotalRunTime;
            return Task.FromResult(new GetAppPackageRunTimeResponse
            {
                Duration = Duration.FromTimeSpan(totalRunTime)
            });
        }
    }
}
