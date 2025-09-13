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
        // Delegate to Sephirah for the actual search
        var sephirahRequest = new TuiHub.Protos.Librarian.Sephirah.V1.SearchStoreAppsRequest
        {
            NameLike = request.NameLike
        };
        
        if (request.Paging != null)
        {
            sephirahRequest.Paging = new TuiHub.Protos.Librarian.V1.PagingRequest
            {
                PageSize = request.Paging.PageSize,
                PageNum = request.Paging.PageNum
            };
        }

        // Forward the authorization header to Sephirah
        var headers = new Metadata();
        if (context.RequestHeaders.FirstOrDefault(h => h.Key == "authorization") is { } authHeader)
        {
            headers.Add("authorization", authHeader.Value);
        }

        try
        {
            var sephirahResponse = await _sephirahClient.SearchStoreAppsAsync(sephirahRequest, headers);
            
            // Convert Sephirah response to Angela response format
            var response = new Librarian.Sephirah.Angela.SearchStoreAppsResponse
            {
                Paging = new Librarian.Sephirah.Angela.PagingResponse
                {
                    TotalSize = sephirahResponse.Paging.TotalSize
                }
            };

            // Convert StoreAppDigest from Sephirah to StoreApp for Angela
            foreach (var digest in sephirahResponse.AppInfos)
            {
                var storeApp = new Librarian.Sephirah.Angela.StoreApp
                {
                    Id = new Librarian.Sephirah.Angela.InternalID { Id = digest.Id.Id },
                    Name = digest.Name,
                    Type = digest.Type.ToString(),
                    Description = digest.ShortDescription,
                    CoverImageId = new Librarian.Sephirah.Angela.InternalID { Id = digest.CoverImageId.Id }
                };
                storeApp.Tags.AddRange(digest.Tags);
                response.StoreApps.Add(storeApp);
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