using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    public override async Task<Librarian.Sephirah.Angela.SearchStoreAppsResponse> SearchStoreApps(Librarian.Sephirah.Angela.SearchStoreAppsRequest request,
        ServerCallContext context)
    {
        // Use AutoMapper to convert request
        var sephirahRequest = _mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.SearchStoreAppsRequest>(request);

        // Forward the authorization header to Sephirah
        var headers = new Metadata();
        if (context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization") is { } authHeader)
        {
            headers.Add("authorization", authHeader.Value);
        }

        try
        {
            var sephirahResponse = await _sephirahClient.SearchStoreAppsAsync(sephirahRequest, headers);
            
            // Use AutoMapper to convert response
            return _mapper.Map<Librarian.Sephirah.Angela.SearchStoreAppsResponse>(sephirahResponse);
        }
        catch (RpcException ex)
        {
            // Re-throw Sephirah errors
            throw new RpcException(new Status(ex.StatusCode, ex.Status.Detail));
        }
    }
}