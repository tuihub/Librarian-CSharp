using Grpc.Core;
using Librarian.Common.Utils;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            var userIdFromJwt = context.GetInternalIdFromHeader();
            // NOTE: request id can be null
            var userIdToGet = request.Id?.Id == null ? userIdFromJwt : request.Id.Id;
            // verify user type if request user id is not same as jwt user id
            if (userIdToGet != userIdFromJwt)
            {
                UserUtil.VerifyUserAdminAndThrow(context, _dbContext, "You don't have permission to get this user.");
            }
            // get user
            var user = _dbContext.Users.SingleOrDefault(u => u.Id == userIdToGet);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "User not exists."));
            }
            return Task.FromResult(new GetUserResponse
            {
                User = user.ToPB()
            });
        }
    }
}
