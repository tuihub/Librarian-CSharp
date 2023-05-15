using Grpc.Core;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            long internalId;
            using var db = new ApplicationDbContext();
            // verify user type(admin)
            if (UserUtil.GetUserTypeFromToken(context, db) != UserType.Admin)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            // create user
            internalId = IdUtil.NewId();
            var user = new Models.User()
            {
                Id = internalId,
                Name = request.User.Username,
                Password = PasswordHasher.HashPassword(request.User.Password),
                Status = UserStatus.Active,
                Type = request.User.Type
            };
            db.Users.Add(user);
            db.SaveChanges();
            return Task.FromResult(new CreateUserResponse()
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID() { Id = internalId }
            });
        }
    }
}
