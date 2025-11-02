using Grpc.Core;
using Librarian.Common.Constants;
using Librarian.Common.Utils;
using Librarian.Sephirah.Angela;
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

        // Get user and verify status and permissions for Angela admin-only access
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == internalId);
        if (user == null)
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));

        if (user.Status != Enums.UserStatus.Active)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active"));

        if (user.Type != Enums.UserType.Admin)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Only administrators can access Angela"));

        // Use AutoMapper to convert request and delegate to Sephirah
        var sephirahRequest = s_mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.RefreshTokenRequest>(request);

        // Forward the authorization header to Sephirah
        var headers = new Metadata();
        if (context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization") is { } authHeader)
            headers.Add("authorization", authHeader.Value);

        try
        {
            var sephirahResponse = await _sephirahClient.RefreshTokenAsync(sephirahRequest, headers);

            // Use AutoMapper to convert response
            return s_mapper.Map<RefreshTokenResponse>(sephirahResponse);
        }
        catch (RpcException ex)
        {
            // Re-throw Sephirah authentication errors
            throw new RpcException(new Status(ex.StatusCode, ex.Status.Detail));
        }
    }
}