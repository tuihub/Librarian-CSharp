using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using ListStoreAppBinariesRequest = Librarian.Sephirah.Angela.ListStoreAppBinariesRequest;
using ListStoreAppBinariesResponse = Librarian.Sephirah.Angela.ListStoreAppBinariesResponse;
using StoreAppBinary = Librarian.Sephirah.Angela.StoreAppBinary;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<ListStoreAppBinariesResponse> ListStoreAppBinaries(ListStoreAppBinariesRequest request,
        ServerCallContext context)
    {
        // Use AutoMapper to convert request
        var sephirahRequest = s_mapper.Map<GetStoreAppSummaryRequest>(request);

        // Forward the authorization header to Sephirah
        var headers = new Metadata();
        if (context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization") is { } authHeader)
            headers.Add("authorization", authHeader.Value);

        try
        {
            var sephirahResponse = await _sephirahClient.GetStoreAppSummaryAsync(sephirahRequest, headers);

            var response = new ListStoreAppBinariesResponse
            {
                Paging = new PagingResponse
                {
                    TotalSize = sephirahResponse.StoreApp.AppBinaryCount
                }
            };

            // Use AutoMapper to convert binaries
            foreach (var sephirahBinary in sephirahResponse.StoreApp.Binaries)
            {
                var binary = s_mapper.Map<StoreAppBinary>(sephirahBinary);
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