using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;

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
            app.UpdateFromPB(appReq);
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppResponse());
        }
    }
}
