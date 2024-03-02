using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // TODO: add normal user support
        [Authorize]
        public override Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            long internalId;
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            // create user
            internalId = _idGenerator.CreateId();
            var user = new Common.Models.User()
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
