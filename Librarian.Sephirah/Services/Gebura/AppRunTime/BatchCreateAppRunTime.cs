using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using DbAppRunTime = Librarian.Common.Models.Db.AppRunTime;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    [Authorize]
    public override Task<BatchCreateAppRunTimeResponse> BatchCreateAppRunTime(BatchCreateAppRunTimeRequest request,
        ServerCallContext context)
    {
        var userId = context.GetInternalIdFromHeader();

        // Validate all AppRunTime in the request
        foreach (var appRunTimeReq in request.AppRunTimes)
        {
            // Check App
            var app = _dbContext.Apps.SingleOrDefault(x => x.Id == appRunTimeReq.AppId.Id);
            if (app == null) throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
            if (app.UserId != userId)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "App is not owned by user."));

            // Check Device
            var device = _dbContext.Devices.Include(x => x.Users)
                .SingleOrDefault(x => x.Id == appRunTimeReq.DeviceId.Id);
            if (device == null) throw new RpcException(new Status(StatusCode.InvalidArgument, "Device not exists."));
            if (!device.Users.Select(x => x.Id).Contains(userId))
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Device is not owned by user."));

            // Add AppRunTime
            app.AppRunTimes.Add(new DbAppRunTime
            {
                StartDateTime = appRunTimeReq.RunTime.StartTime.ToDateTime(),
                EndDateTime =
                    appRunTimeReq.RunTime.StartTime.ToDateTime() + appRunTimeReq.RunTime.Duration.ToTimeSpan(),
                Duration = appRunTimeReq.RunTime.Duration.ToTimeSpan(),
                AppId = app.Id,
                DeviceId = device.Id
            });

            // Update app total runtime
            app.TotalRunTime += appRunTimeReq.RunTime.Duration.ToTimeSpan();
        }

        _dbContext.SaveChanges();
        return Task.FromResult(new BatchCreateAppRunTimeResponse());
    }
}