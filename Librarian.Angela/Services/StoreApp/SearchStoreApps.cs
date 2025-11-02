using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<SearchStoreAppsResponse> SearchStoreApps(SearchStoreAppsRequest request,
        ServerCallContext context)
    {
        // Use AutoMapper to convert request
        var sephirahRequest = s_mapper.Map<TuiHub.Protos.Librarian.Sephirah.V1.SearchStoreAppsRequest>(request);

        // Forward the authorization header to Sephirah
        var headers = new Metadata();
        if (context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization") is { } authHeader)
            headers.Add("authorization", authHeader.Value);

        try
        {
            var sephirahResponse = await _sephirahClient.SearchStoreAppsAsync(sephirahRequest, headers);

            // Use AutoMapper to convert response
            return s_mapper.Map<SearchStoreAppsResponse>(sephirahResponse);
        }
        catch (RpcException ex)
        {
            // Re-throw Sephirah errors
            throw new RpcException(new Status(ex.StatusCode, ex.Status.Detail));
        }
    }
}