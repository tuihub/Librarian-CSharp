using Grpc.Core;
using Librarian.Sephirah.Angela;
using Librarian.Common;
using Librarian.Common.Constants;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(AuthenticationSchemes = "RefreshToken")]
    public override async Task<Librarian.Sephirah.Angela.RefreshTokenResponse> RefreshToken(Librarian.Sephirah.Angela.RefreshTokenRequest request,
        ServerCallContext context)
    {
        var internalId = context.GetInternalIdFromHeader();

        // Get user and verify admin permissions for Angela
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == internalId);
        if (user == null) 
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            
        if (user.Type != Enums.UserType.Admin)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Only administrators can access Angela"));

        // Forward authorization header to Sephirah
        var headers = new Metadata();
        if (context.RequestHeaders.Any(h => h.Key == "authorization"))
        {
            var authHeader = context.RequestHeaders.First(h => h.Key == "authorization");
            headers.Add(authHeader);
        }

        try
        {
            // Delegate to Sephirah service
            var sephirahRequest = new TuiHub.Protos.Librarian.Sephirah.V1.RefreshTokenRequest();
            var sephirahResponse = await _sephirahClient.RefreshTokenAsync(sephirahRequest, headers);

            // Convert response back to Angela format
            return new Librarian.Sephirah.Angela.RefreshTokenResponse
            {
                AccessToken = sephirahResponse.AccessToken,
                RefreshToken = sephirahResponse.RefreshToken
            };
        }
        catch (RpcException)
        {
            // Re-throw RPC exceptions as-is
            throw;
        }
    }
}