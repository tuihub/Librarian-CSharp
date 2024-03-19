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
        public override Task<CreateAppInfoResponse> CreateAppInfo(CreateAppInfoRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            // create appInfo
            if (!request.AppInfo.Internal)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfoSource must be internal."));
            }
            var internalId = _idGenerator.CreateId();
            var appInfo = new Common.Models.Db.AppInfo(internalId, request.AppInfo);
            _dbContext.AppInfos.Add(appInfo);
            _dbContext.SaveChanges();
            return Task.FromResult(new CreateAppInfoResponse
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = internalId }
            });
        }
    }
}
