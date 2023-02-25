using Grpc.Core;
using Librarian.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Services.Sephirah
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            long internalId;
            try
            {
                internalId = IdUtil.NewId();
                using var db = new TestDbContext();
                // verify user type(admin)
                var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
                var internalIdFromToken = JwtUtil.GetInternalIdFromToken(token);
                var userFromToken = db.Users.Single(x => x.InternalId == internalIdFromToken);
                if (userFromToken.Type != UserType.Admin)
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
                // create user
                var user = new Models.User()
                {
                    InternalId = internalId,
                    Name = request.User.Username,
                    Password = PasswordHasher.HashPassword(request.User.Password),
                    Status = UserStatus.Active,
                    Type = request.User.Type,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = null
                };
                db.Users.Add(user);
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
            return Task.FromResult(new CreateUserResponse()
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID() { Id = internalId }
            });
        }
    }
}
