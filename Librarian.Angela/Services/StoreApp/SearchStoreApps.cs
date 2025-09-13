using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    public override async Task<Librarian.Sephirah.Angela.SearchStoreAppsResponse> SearchStoreApps(Librarian.Sephirah.Angela.SearchStoreAppsRequest request,
        ServerCallContext context)
    {
        // Convert Angela request to Sephirah request
        var sephirahRequest = new TuiHub.Protos.Librarian.Sephirah.V1.SearchStoreAppsRequest
        {
            NameLike = request.NameLike,
            Paging = request.Paging != null ? new TuiHub.Protos.Librarian.V1.PagingRequest
            {
                PageSize = request.Paging.PageSize,
                PageNum = request.Paging.PageNum
            } : null
        };

        // Forward authorization header
        var headers = new Metadata();
        if (context.RequestHeaders.Any(h => h.Key == "authorization"))
        {
            var authHeader = context.RequestHeaders.First(h => h.Key == "authorization");
            headers.Add(authHeader);
        }

        // Call Sephirah service
        var sephirahResponse = await _sephirahClient.SearchStoreAppsAsync(sephirahRequest, headers);

        // Convert Sephirah response to Angela response
        var response = new Librarian.Sephirah.Angela.SearchStoreAppsResponse
        {
            Paging = new Librarian.Sephirah.Angela.PagingResponse
            {
                TotalSize = sephirahResponse.Paging?.TotalSize ?? 0
            }
        };

        foreach (var sephirahStoreApp in sephirahResponse.AppInfos)
        {
            var angelaStoreApp = new Librarian.Sephirah.Angela.StoreApp
            {
                Id = new Librarian.Sephirah.Angela.InternalID { Id = sephirahStoreApp.Id.Id },
                Name = sephirahStoreApp.Name,
                Type = sephirahStoreApp.Type.ToString(),
                Description = sephirahStoreApp.ShortDescription,
                CoverImageId = new Librarian.Sephirah.Angela.InternalID { Id = sephirahStoreApp.CoverImageId.Id }
            };
            angelaStoreApp.Tags.AddRange(sephirahStoreApp.Tags);
            response.StoreApps.Add(angelaStoreApp);
        }

        return response;
    }
}