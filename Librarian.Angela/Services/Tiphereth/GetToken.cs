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
        // First verify user exists and is admin using Angela's requirement
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == request.Username);
        if (user == null) throw new RpcException(new Status(StatusCode.NotFound, "User does not exist"));

        // Verify user is active
        if (user.Status != Enums.UserStatus.Active)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active"));

        // Verify that the user is an administrator for Angela access
        if (user.Type != Enums.UserType.Admin)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "Only administrators can access Angela"));

        // Delegate to Sephirah for actual token generation
        var sephirahRequest = new TuiHub.Protos.Librarian.Sephirah.V1.GetTokenRequest
        {
            Username = request.Username,
            Password = request.Password
        };

        try
        {
            var sephirahResponse = await _sephirahClient.GetTokenAsync(sephirahRequest);
            
            return new Librarian.Sephirah.Angela.GetTokenResponse
            {
                AccessToken = sephirahResponse.AccessToken,
                RefreshToken = sephirahResponse.RefreshToken
            };
        }
        catch (RpcException ex)
        {
            // Re-throw Sephirah authentication errors
            throw new RpcException(new Status(ex.StatusCode, ex.Status.Detail));
        }
    }
}