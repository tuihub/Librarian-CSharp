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
        public override Task<UnAssignAppResponse> UnAssignApp(UnAssignAppRequest request, ServerCallContext context)
        {
            var appId = request.AppId.Id;
            var app = _dbContext.Apps.SingleOrDefault(x => x.Id == appId);
            if (app == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
            }
            app.AppInfoId = null;
            _dbContext.SaveChanges();
            return Task.FromResult(new UnAssignAppResponse());
        }
    }
}
