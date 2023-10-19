using Grpc.Core;
using Librarian.Common.Utils;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            var userId = JwtUtil.GetInternalIdFromJwt(context);
            // NOTE: request id can be null
            if (request.Id?.Id != null && request.Id.Id != userId)
            {
                // verify user type(admin)
                if (UserUtil.GetUserTypeFromJwt(context, _dbContext) != UserType.Admin)
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            }
            // get user
            var userIdToGet = request.Id?.Id == null ? userId : request.Id.Id;
            var user = _dbContext.Users.SingleOrDefault(u => u.Id == userIdToGet);
            if (user == null)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "User not exists."));
            return Task.FromResult(new GetUserResponse
            {
                User = user.ToProtoUser()
            });
        }
    }
}
