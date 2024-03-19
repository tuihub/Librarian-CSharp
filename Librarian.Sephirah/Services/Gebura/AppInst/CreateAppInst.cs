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
        public override Task<CreateAppInstResponse> CreateAppInst(CreateAppInstRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var appInstReq = request.AppInst;
            var internalId = _idGenerator.CreateId();
            var app = _dbContext.Apps.SingleOrDefault(x => x.Id == appInstReq.AppId.Id);
            if (app == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
            }
            else if (app.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "App not owned by user."));
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
            var appInst = new Common.Models.Db.AppInst(internalId, appInstReq);
            _dbContext.AppInsts.Add(appInst);
            _dbContext.SaveChanges();
            return Task.FromResult(new CreateAppInstResponse
            {
                Id = new InternalID { Id = internalId }
            });
        }
    }
}
