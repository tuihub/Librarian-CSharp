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
        public override Task<CreateAppResponse> CreateApp(CreateAppRequest request, ServerCallContext context)
        {
            // create app
            var internalId = _idGenerator.CreateId();
            var app = new Common.Models.Db.App(internalId, request.App);
            if (app.AppInfoId != null)
            {
                var appInfo = _dbContext.AppInfos.SingleOrDefault(x => x.Id == app.AppInfoId);
                if (appInfo == null)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "AssignedAppInfo not exists."));
                }
                appInfo.Apps.Add(app);
            }
            _dbContext.SaveChanges();
            return Task.FromResult(new CreateAppResponse
            {
                Id = new InternalID { Id = internalId }
            });
        }
    }
}
