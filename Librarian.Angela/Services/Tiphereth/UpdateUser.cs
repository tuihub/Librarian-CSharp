using Grpc.Core;
using Librarian.Sephirah.Angela;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        // Verify that the user is an administrator
        UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

        var userId = request.User.Id.Id;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) throw new RpcException(new Status(StatusCode.NotFound, "User does not exist"));

        // Update user information
        if (!string.IsNullOrEmpty(request.User.Username)) user.Name = request.User.Username;

        if (!string.IsNullOrEmpty(request.User.Password))
            user.Password = request.User.Password; // Note: In a real project, the password should be stored encrypted

        if (request.User.Type != UserType.Unspecified) user.Type = ConvertToDbUserType(request.User.Type);

        if (request.User.Status != UserStatus.Unspecified) user.Status = ConvertToDbUserStatus(request.User.Status);

        user.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return new UpdateUserResponse();
    }
}