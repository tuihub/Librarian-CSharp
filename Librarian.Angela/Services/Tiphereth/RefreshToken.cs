using Grpc.Core;
using Librarian.Angela.V1;
using Librarian.Common;
using Librarian.Common.Constants;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(AuthenticationSchemes = "RefreshToken")]
    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request,
        ServerCallContext context)
    {
        var internalId = context.GetInternalIdFromHeader();

        // Get user and verify administrator status
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == internalId);
        if (user == null || user.Type != Enums.UserType.Admin)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Only administrators can refresh tokens"));

        // Get new tokens
        var accessToken = JwtUtil.GenerateAccessToken(internalId);
        var refreshToken = context.GetBearerToken()!;
        var expireTime = JwtUtil.GetTokenExpireTime(refreshToken);

        // Only refresh the token when the expiration time is less than 25% of the refresh token expiration time
        if (DateTime.UtcNow + TimeSpan.FromMinutes(GlobalContext.JwtConfig.RefreshTokenExpireMinutes * 0.25) >
            expireTime) refreshToken = JwtUtil.GenerateRefreshToken(internalId);

        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}