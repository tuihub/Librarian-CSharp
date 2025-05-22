using Grpc.Core;
using Librarian.Common.Constants;
using Librarian.Common.Converters;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using TuiHub.Protos.Librarian.Sephirah.V1.Angela;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Services
{
    public partial class AngelaService
    {
        [Authorize]
        [EnableRateLimiting("bcrypt_fixed")]
        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            // Verify that the user is an administrator
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

            // Generate a new internal ID
            long internalId = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Create a new user
            var user = new Common.Models.Db.User()
            {
                Id = internalId,
                Name = request.User.Username,
                Password = request.User.Password, // Note: In a real project, the password should be encrypted
                Status = Enums.UserStatus.Active,
                Type = request.User.Type.ToString().ToEnum<Enums.UserType>()
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return new CreateUserResponse()
            {
                Id = new InternalID() { Id = internalId }
            };
        }
    }
}
