using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<AssignAppResponse> AssignApp(AssignAppRequest request, ServerCallContext context)
        {
            var appInfoId = request.AppInfoId.Id;
            var appId = request.AppId.Id;
            var appInfo = _dbContext.AppInfos.SingleOrDefault(x => x.Id == appInfoId);
            if (appInfo == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfo not exists."));
            }
            if (appInfo.IsInternal == false)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfoSource must be internal."));
            }
            var app = _dbContext.Apps.SingleOrDefault(x => x.Id == appId);
            if (app == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
            }
            app.AppInfoId = appInfoId;
            _dbContext.SaveChanges();
            return Task.FromResult(new AssignAppResponse());
        }
    }
}
