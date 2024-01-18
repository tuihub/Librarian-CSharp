using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            // update user
            var userReq = request.User;
            var user = _dbContext.Users.SingleOrDefault(x => x.Id == userReq.Id.Id);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "User not exists."));
            }
            if (userReq.Username.Length > 0) { user.Name = userReq.Username; }
            if (userReq.Password.Length > 0) { user.Password = PasswordHasher.HashPassword(userReq.Password); }
            if (userReq.Type != UserType.Unspecified) { user.Type = userReq.Type; }
            if (userReq.Status != UserStatus.Unspecified) { user.Status = userReq.Status; }
            user.UpdatedAt = DateTime.Now;
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateUserResponse());
        }
    }
}
