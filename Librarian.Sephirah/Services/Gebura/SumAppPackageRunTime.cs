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
        public override Task<SumAppPackageRunTimeResponse> SumAppPackageRunTime(SumAppPackageRunTimeRequest request, ServerCallContext context)
        {
            var userId = JwtUtil.GetInternalIdFromJwt(context);
            var appPackageId = request.AppPackageId.Id;
            var timeAggregation = request.TimeAggregation;
            if (timeAggregation.AggregationType != TimeAggregation.Types.AggregationType.Overall)
                throw new RpcException(new Status(StatusCode.Unimplemented, "Only support overall aggregation type."));
            var totalRunTime = _dbContext.UserAppPackages
                                         .SingleOrDefault(x => x.UserId == userId &&
                                                      x.AppPackageId == appPackageId)
                                         ?.TotalRunTime;
            if (totalRunTime == null)
            {
                _dbContext.UserAppPackages.Add(new UserAppPackage
                {
                    UserId = userId,
                    AppPackageId = appPackageId,
                    TotalRunTime = TimeSpan.Zero
                });
                _dbContext.SaveChanges();
                totalRunTime = TimeSpan.Zero;
            }
            var ret = new SumAppPackageRunTimeResponse();
            ret.RunTimeGroups.Add(new SumAppPackageRunTimeResponse.Types.Group
            {
                Duration = Duration.FromTimeSpan((TimeSpan)totalRunTime)
            });
            return Task.FromResult(ret);
        }
    }
}
