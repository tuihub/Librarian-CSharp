using Grpc.Core;
using Librarian.Common.Utils;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            var userIdFromJwt = context.GetInternalIdFromHeader();
            var userType = UserUtil.GetUserTypeFromJwt(context, _dbContext);
            // NOTE: request id can be null
            var userIdToGet = request.Id?.Id == null ? userIdFromJwt : request.Id.Id;
            if (userType != UserType.Admin && userIdFromJwt != userIdToGet)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "You don't have permission to get this user."));
            }
            // get user
            var user = _dbContext.Users.SingleOrDefault(u => u.Id == userIdToGet);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "User not exists."));
            }
            return Task.FromResult(new GetUserResponse
            {
                User = user.ToProtoUser()
            });
        }
    }
}
