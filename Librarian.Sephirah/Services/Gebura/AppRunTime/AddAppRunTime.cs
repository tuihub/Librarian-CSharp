using Grpc.Core;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<AddAppRunTimeResponse> AddAppRunTime(AddAppRunTimeRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var timeRangeReq = request.TimeRange;
            // check app
            var app = _dbContext.Apps.SingleOrDefault(x => x.Id == request.AppId.Id);
            if (app == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
            }
            if (app.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "App is not owned by user."));
            }
            // check device
            var device = _dbContext.Devices.Include(x => x.Users).SingleOrDefault(x => x.Id == request.DeviceId.Id);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Device not exists."));
            }
            if (!device.Users.Select(x => x.Id).Contains(userId))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Device is not owned by user."));
            }
            // add AppRunTime
            app.AppRunTimes.Add(new AppRunTime
            {
                StartDateTime = timeRangeReq.StartTime.ToDateTime(),
                EndDateTime = timeRangeReq.StartTime.ToDateTime() + timeRangeReq.Duration.ToTimeSpan(),
                Duration = timeRangeReq.Duration.ToTimeSpan(),
                AppId = app.Id,
                DeviceId = device.Id
            });
            // add to App TotalRunTime
            app.TotalRunTime += timeRangeReq.Duration.ToTimeSpan();
            _dbContext.SaveChanges();
            return Task.FromResult(new AddAppRunTimeResponse());
        }
    }
}
