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
        public override Task<UpdateAppResponse> UpdateApp(UpdateAppRequest request, ServerCallContext context)
        {
            // check App exists
            var appReq = request.App;
            var app = _dbContext.Apps.SingleOrDefault(x => x.Id == appReq.Id.Id);
            if (app == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
            }
            // update App
            app.UpdateFromProtoApp(appReq);
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppResponse());
        }
    }
}
