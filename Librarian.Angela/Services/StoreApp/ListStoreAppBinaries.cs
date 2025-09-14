using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    public override async Task<Librarian.Sephirah.Angela.ListStoreAppBinariesResponse> ListStoreAppBinaries(Librarian.Sephirah.Angela.ListStoreAppBinariesRequest request,
        ServerCallContext context)
    {
        // Use AutoMapper to convert request
        var sephirahRequest = _mapper.Map<GetStoreAppSummaryRequest>(request);

        // Forward the authorization header to Sephirah
        var headers = new Metadata();
        if (context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization") is { } authHeader)
        {
            headers.Add("authorization", authHeader.Value);
        }

        try
        {
            var sephirahResponse = await _sephirahClient.GetStoreAppSummaryAsync(sephirahRequest, headers);
            
            var response = new Librarian.Sephirah.Angela.ListStoreAppBinariesResponse
            {
                Paging = new Librarian.Sephirah.Angela.PagingResponse
                {
                    TotalSize = sephirahResponse.StoreApp.AppBinaryCount
                }
            };

            // Use AutoMapper to convert binaries
            foreach (var sephirahBinary in sephirahResponse.StoreApp.Binaries)
            {
                var binary = _mapper.Map<Librarian.Sephirah.Angela.StoreAppBinary>(sephirahBinary);
                // Set required fields that AutoMapper can't infer
                binary.StoreAppId = request.StoreAppId;
                response.Binaries.Add(binary);
            }

            return response;
        }
        catch (RpcException ex)
        {
            // Re-throw Sephirah errors
            throw new RpcException(new Status(ex.StatusCode, ex.Status.Detail));
        }
    }
}