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
        public override Task<UpdateAppInfoResponse> UpdateAppInfo(UpdateAppInfoRequest request, ServerCallContext context)
        {
            // ensure AppInfoSource == internal
            var appInfoReq = request.AppInfo;
            if (!appInfoReq.Internal)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfoSource must be internal."));
            }
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            // check AppInfo exists
            var appInfo = _dbContext.AppInfos.Include(x => x.AppInfoDetails)
                .SingleOrDefault(x => x.Id == appInfoReq.Id.Id);
            if (appInfo == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfo not exists."));
            }
            // update AppInfo, full update
            appInfo.UpdateFromProtoAppInfo(appInfoReq);
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppInfoResponse());
        }
    }
}
