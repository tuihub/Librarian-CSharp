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
            long internalIdNew;
            try
            {
                internalIdNew = IdUtil.NewId();
                using var db = new TestDbContext();
                // verify user type(admin)
                var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
                var internalId = JwtUtil.GetInternalIdFromToken(token);
                var user = db.Users.Single(x => x.InternalId == internalId);
                if (user.Type != UserType.Admin)
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
                // create user
                var userNew = new Models.User()
                {
                    InternalId = internalIdNew,
                    Name = request.User.Username,
                    Password = PasswordHasher.HashPassword(request.User.Password),
                    Status = Models.User.StatusEnum.Activated,
                    Type = request.User.Type,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                db.Users.Add(userNew);
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
                Id = new TuiHub.Protos.Librarian.V1.InternalID() { Id = internalIdNew }
            });
        }
    }
}
