using Grpc.Core;
using Librarian.Sephirah.Angela;
using Librarian.Common.Constants;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    public override async Task<GetTokenResponse> GetToken(GetTokenRequest request, ServerCallContext context)
    {
        // Find user by username
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Username);
        if (user == null) throw new RpcException(new Status(StatusCode.NotFound, "User does not exist"));

        // Verify user is active
        if (user.Status != Enums.UserStatus.Active)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active"));

        // Verify password using proper hashing
        if (!PasswordHasher.VerifyHashedPassword(user.Password, request.Password))
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Username and password not match"));

        // Verify that the user is an administrator for Angela access
        if (user.Type != Enums.UserType.Admin)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Only administrators can access Angela"));

        // Generate tokens
        var accessToken = JwtUtil.GenerateAccessToken(user.Id);
        var refreshToken = JwtUtil.GenerateRefreshToken(user.Id);

        return new GetTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}