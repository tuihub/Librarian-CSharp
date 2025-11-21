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
    [Authorize(AuthenticationSchemes = "RefreshToken", Policy = "AngelaAccess")]
    public override async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request,
        ServerCallContext context)
    {
        string accessTokenNew, refreshTokenNew;
        var internalId = context.GetInternalIdFromHeader();

        // Get user and verify status and permissions for Angela admin-only access
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Id == internalId);
        if (user == null)
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        if (user.Status != Enums.UserStatus.Active)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active"));

        // get new token
        accessTokenNew = JwtUtil.GenerateAccessToken(internalId);
        refreshTokenNew = JwtUtil.GenerateRefreshToken(internalId);

        return new RefreshTokenResponse
        {
            AccessToken = accessTokenNew,
            RefreshToken = refreshTokenNew
        };
    }
}