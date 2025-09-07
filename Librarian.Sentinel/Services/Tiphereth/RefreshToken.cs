using Grpc.Core;
using Librarian.Common;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sentinel.V1;

namespace Librarian.Sentinel.Services;

public partial class SephirahSentinelService
{
    // TODO: impl revoke
    [Authorize(AuthenticationSchemes = "RefreshToken")]
    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request,
        ServerCallContext context)
    {
        var internalId = context.GetInternalIdFromHeader();
        // get sentinel
        if (!await _dbContext.Sentinels.AnyAsync(x => x.Id == internalId))
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Sentinel not exists"));
        // get new token
        var accessToken = JwtUtil.GenerateSentinelAccessToken(internalId);
        var refreshToken = context.GetBearerToken()!;
        var expireTime = JwtUtil.GetTokenExpireTime(refreshToken);
        // refresh token only when expire time is less than 25% of the refresh token expire time
        if (DateTime.UtcNow + TimeSpan.FromMinutes(GlobalContext.JwtConfig.SentinelRefreshTokenExpireMinutes * 0.25) >
            expireTime) refreshToken = JwtUtil.GenerateSentinelRefreshToken(internalId);
        // return new tokens
        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}