using Grpc.Core;
using Librarian.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Services.Sephirah
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            try
            {
                using var db = new TestDbContext();
                // verify user type(admin)
                var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
                var internalIdFromToken = JwtUtil.GetInternalIdFromToken(token);
                var userFromToken = db.Users.Single(x => x.InternalId == internalIdFromToken);
                if (userFromToken.Type != UserType.Admin)
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
                db.SaveChanges();
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, e.Message));
            }
            return Task.FromResult(new UpdateUserResponse());
        }
    }
}
