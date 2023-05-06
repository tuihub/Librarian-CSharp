using Grpc.Core;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            using var db = new ApplicationDbContext();
            // verify user type(admin)
            if (UserUtil.GetUserTypeFromToken(context, db) != UserType.Admin)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            // update user
            var userReq = request.User;
            var user = db.Users.SingleOrDefault(x => x.InternalId == userReq.Id.Id);
            if (user == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "User not exists."));
            if (userReq.Username.Length > 0) user.Name = userReq.Username;
            if (userReq.Password.Length > 0) user.Password = PasswordHasher.HashPassword(userReq.Password);
            if (userReq.Type != UserType.Unspecified) user.Type = userReq.Type;
            if (userReq.Status != UserStatus.Unspecified) user.Status = userReq.Status;
            user.UpdatedAt = DateTime.Now;
            db.SaveChanges();
            return Task.FromResult(new UpdateUserResponse());
        }
    }
}
