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

        // Use AutoMapper to convert request and delegate to Sephirah
        var sephirahRequest = s_mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.GetTokenRequest>(request);

        try
        {
            var sephirahResponse = await _sephirahClient.GetTokenAsync(sephirahRequest);
            
            // Use AutoMapper to convert response
            return s_mapper.Map<Librarian.Sephirah.Angela.GetTokenResponse>(sephirahResponse);
        }
        catch (RpcException ex)
        {
            // Re-throw Sephirah authentication errors
            throw new RpcException(new Status(ex.StatusCode, ex.Status.Detail));
        }
    }
}