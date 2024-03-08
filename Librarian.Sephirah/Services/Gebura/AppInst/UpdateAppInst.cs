using Grpc.Core;
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
        public override Task<UpdateAppInstResponse> UpdateAppInst(UpdateAppInstRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var appInstReq = request.AppInst;
            var appInst = _dbContext.AppInsts
                .Where(x => x.Id == appInstReq.Id.Id)
                .Include(x => x.App)
                .SingleOrDefault(x => x.Id == appInstReq.Id.Id);
            if (appInst == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Original AppInst not exists."));
            }
            if (appInst.App == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Original AppInst associcated App not exists."));
            }
            else if (appInst.App.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Original AppInst associcated App not owned by user."));
            }
            var device = _dbContext.Devices
                .Where(x => x.Id == appInstReq.DeviceId.Id)
                .Include(x => x.Users)
                .SingleOrDefault(x => x.Id == appInstReq.DeviceId.Id);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Device not exists."));
            }
            if (!device.Users.Any(x => x.Id == userId))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Device not associated with user."));
            }
            // get appReq App
            var app = _dbContext.Apps.SingleOrDefault(x => x.Id == appInstReq.AppId.Id);
            if (app == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "App from request not exists."));
            }
            if (app.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "App from request is not owned by user."));
            }
            // update TotalRunTime
            appInst.App.TotalRunTime -= appInst.TotalRunTime;
            app.TotalRunTime += appInst.TotalRunTime;
            // update AppInst
            appInst.App = app;
            appInst.DeviceId = appInstReq.DeviceId.Id;
            appInst.UpdatedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppInstResponse());
        }
    }
}
