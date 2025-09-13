using Grpc.Core;
using Librarian.Sephirah.Angela;
using Librarian.Common.Constants;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    public override async Task<Librarian.Sephirah.Angela.GetTokenResponse> GetToken(Librarian.Sephirah.Angela.GetTokenRequest request, ServerCallContext context)
    {
        // First verify that the user is an administrator for Angela access
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Username);
        if (user == null)
            throw new RpcException(new Status(StatusCode.NotFound, "User does not exist"));

        if (user.Type != Enums.UserType.Admin)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Only administrators can access Angela"));

        // Convert Angela request to Sephirah request
        var sephirahRequest = new TuiHub.Protos.Librarian.Sephirah.V1.GetTokenRequest
        {
            Username = request.Username,
            Password = request.Password
        };

        try
        {
            // Delegate to Sephirah service for token generation
            var sephirahResponse = await _sephirahClient.GetTokenAsync(sephirahRequest);

            // Convert response back to Angela format
            return new Librarian.Sephirah.Angela.GetTokenResponse
            {
                AccessToken = sephirahResponse.AccessToken,
                RefreshToken = sephirahResponse.RefreshToken
            };
        }
        catch (RpcException)
        {
            // Re-throw RPC exceptions (like authentication failures) as-is
            throw;
        }
    }
}