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
            // verify user type(admin)
            if (UserUtil.GetUserTypeFromJwt(context, _dbContext) != UserType.Admin)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            // create app
            var internalId = _idGenerator.CreateId();
            if (request.App.Source != AppSource.Internal)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppSource must be APP_SOURCE_INTERNAL."));
            var app = new Common.Models.App(internalId, request.App);
            _dbContext.Apps.Add(app);
            _dbContext.SaveChanges();
            return Task.FromResult(new CreateAppResponse
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = internalId }
            });
        }
    }
}
