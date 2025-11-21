using Grpc.Core;
using Librarian.Common.Constants;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<GetTokenResponse> GetToken(GetTokenRequest request, ServerCallContext context)
    {
        string accessToken, refreshToken;
        var username = request.Username;
        var password = request.Password;

        // First verify user exists and is admin using Angela's requirement
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Name == username);
        if (user == null) throw new RpcException(new Status(StatusCode.NotFound, "User does not exist"));

        // Verify user is active
        if (user.Status != Enums.UserStatus.Active)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active"));

        // Verify that the user is an administrator for Angela access
        if (user.Type != Enums.UserType.Admin)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Only administrators can access Angela"));

        if (PasswordHasher.VerifyHashedPassword(user.Password, password))
        {
            // get token
            accessToken = JwtUtil.GenerateAccessToken(user.Id);
            refreshToken = JwtUtil.GenerateRefreshToken(user.Id);
        }
        else
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Username and password not match."));
        }

        return new GetTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}