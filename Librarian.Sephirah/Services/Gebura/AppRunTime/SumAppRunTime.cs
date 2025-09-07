using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    [Authorize]
    public override Task<SumAppRunTimeResponse> SumAppRunTime(SumAppRunTimeRequest request, ServerCallContext context)
    {
        var userId = context.GetInternalIdFromHeader();

        // Check AppIdFilter
        var appIds = request.AppIdFilter.Select(x => x.Id);
        if (_dbContext.Apps.Any(x => appIds.Contains(x.Id) && x.UserId != userId))
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                "AppIdFilter contains app not owned by user."));
        // Check DeviceIdFilter
        var deviceIds = request.DeviceIdFilter.Select(x => x.Id);
        var userDeviceIds = _dbContext.Users.Include(x => x.Devices).Single(x => x.Id == userId).Devices
            .Select(x => x.Id);
        if (deviceIds.Except(userDeviceIds).Any())
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                "DeviceIdFilter contains device not owned by user."));

        // Calculate total runtime
        var query = _dbContext.AppRunTimes.AsQueryable();
        if (appIds.Any()) query = query.Where(x => appIds.Contains(x.AppId));
        if (deviceIds.Any()) query = query.Where(x => deviceIds.Contains(x.DeviceId));
        if (request.TimeRangeCross != null)
        {
            var startTime = request.TimeRangeCross.StartTime.ToDateTime();
            var endTime = startTime + request.TimeRangeCross.Duration.ToTimeSpan();
            query = query.Where(x => x.StartDateTime >= startTime && x.StartDateTime <= endTime);
        }

        var runTimeSum = query.Aggregate(TimeSpan.Zero, (acc, x) => acc + x.Duration);

        return Task.FromResult(new SumAppRunTimeResponse
        {
            RunTimeSum = runTimeSum.ToDuration()
        });
    }
}