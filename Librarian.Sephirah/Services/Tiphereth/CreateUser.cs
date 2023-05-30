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
            // verify user type(admin)
            if (UserUtil.GetUserTypeFromJwt(context, _dbContext) != UserType.Admin)
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
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return Task.FromResult(new CreateUserResponse()
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID() { Id = internalId }
            });
        }
    }
}
