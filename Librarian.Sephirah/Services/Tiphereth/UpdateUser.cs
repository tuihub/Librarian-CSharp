using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            // get user type
            var userType = UserUtil.GetUserTypeFromJwt(context, _dbContext);
            var userIdFromJwt = context.GetInternalIdFromHeader();
            // update user
            var userReq = request.User;
            if (userType != UserType.Admin && userIdFromJwt != userReq.Id.Id)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "You don't have permission to update this user."));
            }
            var user = _dbContext.Users.SingleOrDefault(x => x.Id == userReq.Id.Id);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "User not exists."));
            }
            if (!string.IsNullOrEmpty(userReq.Username)) { user.Name = userReq.Username; }
            if (!string.IsNullOrEmpty(userReq.Password)) { user.Password = PasswordHasher.HashPassword(userReq.Password); }
            if (userReq.Type != UserType.Unspecified) { user.Type = userReq.Type; }
            if (userReq.Status != UserStatus.Unspecified) { user.Status = userReq.Status; }
            user.UpdatedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateUserResponse());
        }
    }
}
