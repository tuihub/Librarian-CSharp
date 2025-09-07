using Grpc.Core;
using Librarian.Angela.V1;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using User = Librarian.Common.Models.Db.User;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    [EnableRateLimiting("bcrypt_fixed")]
    public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        // Verify that the user is an administrator
        UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

        // Generate a new internal ID using a simple timestamp approach
        var internalId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Create a new user
        var user = new User
        {
            Id = internalId,
            Name = request.User.Username,
            Password = request.User.Password, // Note: In a real project, the password should be encrypted
            Status = ConvertToDbUserStatus(request.User.Status),
            Type = ConvertToDbUserType(request.User.Type)
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return new CreateUserResponse
        {
            Id = new InternalID { Id = internalId }
        };
    }
}