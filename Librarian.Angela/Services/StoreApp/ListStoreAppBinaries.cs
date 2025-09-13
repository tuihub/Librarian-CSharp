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
        // For binaries, we'll use GetStoreAppSummary with binary limit
        // Note: This doesn't support pagination exactly like the original, but provides the binaries
        var pageSize = (int)(request.Paging?.PageSize ?? 10);
        
        var sephirahRequest = new GetStoreAppSummaryRequest
        {
            StoreAppId = new TuiHub.Protos.Librarian.V1.InternalID { Id = request.StoreAppId.Id },
            AppBinaryLimit = pageSize,
            AppSaveFileLimit = 0,
            AcquiredUserLimit = 0
        };

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

            // Convert Sephirah StoreAppBinary to Angela StoreAppBinary format
            foreach (var sephirahBinary in sephirahResponse.StoreApp.Binaries)
            {
                var binary = new Librarian.Sephirah.Angela.StoreAppBinary
                {
                    Id = new Librarian.Sephirah.Angela.InternalID { Id = sephirahBinary.Id.Id },
                    Name = sephirahBinary.Name,
                    // Note: Sephirah binary format is different, we may need to adjust this
                    // based on what fields are actually available
                    SentinelId = new Librarian.Sephirah.Angela.InternalID { Id = 0 }, // Default value, adjust as needed
                    SentinelGeneratedId = "", // Default value, adjust as needed
                    StoreAppId = new Librarian.Sephirah.Angela.InternalID { Id = request.StoreAppId.Id }
                };
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