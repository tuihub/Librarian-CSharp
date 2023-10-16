using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
            var app = _dbContext.Apps.Include(x => x.ChildApps)
                                     .Single(x => x.Id == appId);
            if (app.Source != TuiHub.Protos.Librarian.V1.AppSource.Internal)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Cannot refresh not internal app."));
            // refresh app
            foreach (var childApp in app.ChildApps)
                if (childApp.Source == TuiHub.Protos.Librarian.V1.AppSource.Steam ||
                    childApp.Source == TuiHub.Protos.Librarian.V1.AppSource.Vndb ||
                    childApp.Source == TuiHub.Protos.Librarian.V1.AppSource.Bangumi)
                    _pullMetadataService.AddPullApp(childApp.Id);
            return Task.FromResult(new RefreshAppResponse { });
        }
    }
}
