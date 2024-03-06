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
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInst not exists."));
            }
            if (appInst.App == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
            }
            else if (appInst.App.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "App not owned by user."));
            }
            var device = _dbContext.Devices.Include(x => x.Users)
                .SingleOrDefault(x => x.Id == appInstReq.DeviceId.Id);
            if (device == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Device not exists."));
            }
            if (!device.Users.Any(x => x.Id == userId))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Device not associated with user."));
            }
            // update TotalRunTime
            _dbContext.UserApps.Single(x => x.UserId == userId && x.AppId == appInst.AppId).TotalRunTime -= appInst.TotalRunTime;
            _dbContext.UserApps.Single(x => x.UserId == userId && x.AppId == appInstReq.AppId.Id).TotalRunTime += appInst.TotalRunTime;
            appInst.AppId = appInstReq.AppId.Id;
            appInst.DeviceId = appInstReq.DeviceId.Id;
            appInst.UpdatedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppInstResponse());
        }
    }
}
