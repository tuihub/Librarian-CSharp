using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<RefreshAppResponse> RefreshApp(RefreshAppRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            if (UserUtil.GetUserTypeFromJwt(context, _dbContext) != UserType.Admin)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            // get request param
            var appId = request.AppId.Id;
            var app = _dbContext.Apps.Single(x => x.Id == appId);
            if (app.Source == TuiHub.Protos.Librarian.V1.AppSource.Internal)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Cannot refresh internal app."));
            // refresh app
            _pullMetadataService.AddPullApp(appId);
            return Task.FromResult(new RefreshAppResponse { });
        }
    }
}
