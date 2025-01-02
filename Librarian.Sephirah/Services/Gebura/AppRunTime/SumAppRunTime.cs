using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    // TODO: Implement TimeAggregation
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<SumAppRunTimeResponse> SumAppRunTime(SumAppRunTimeRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            if (request.TimeAggregation.AggregationType != TimeAggregation.Types.AggregationType.Overall)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Only AggregationType.Overall is supported."));
            }
            // check AppIdFilter
            if (request.AppIdFilter.Count == 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppIdFilter is required."));
            }
            var appIds = request.AppIdFilter.Select(x => x.Id);
            if (_dbContext.Apps.Any(x => appIds.Contains(x.Id) && x.UserId != userId))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "AppIdFilter contains app not owned by user."));
            }
            // check DeviceIdFilter
            var deviceIds = request.DeviceIdFilter.Select(x => x.Id);
            var userDeviceIds = _dbContext.Users.Include(x => x.Devices).Single(x => x.Id == userId).Devices.Select(x => x.Id);
            if (deviceIds.Except(userDeviceIds).Any())
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "DeviceIdFilter contains device not owned by user."));
            }
            // sum
            TimeSpan totalRunTime;
            if (!deviceIds.Any())
            {
                totalRunTime = _dbContext.Apps.Where(x => appIds.Contains(x.Id)).Aggregate(TimeSpan.Zero, (acc, x) => acc + x.TotalRunTime);
            }
            else
            {
                totalRunTime = _dbContext.AppRunTimes.Where(x => appIds.Contains(x.AppId) && deviceIds.Contains(x.DeviceId))
                    .Aggregate(TimeSpan.Zero, (acc, x) => acc + x.Duration);
            }
            return Task.FromResult(new SumAppRunTimeResponse
            {
                RunTimeGroups = { new SumAppRunTimeResponse.Types.Group { Duration = totalRunTime.ToDuration() } }
            });
        }
    }
}
