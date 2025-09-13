using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    public override async Task<GetStoreAppResponse> GetStoreApp(GetStoreAppRequest request,
        ServerCallContext context)
    {
        // Delegate to Sephirah GetStoreAppSummary for the actual data
        var sephirahRequest = new GetStoreAppSummaryRequest
        {
            StoreAppId = new TuiHub.Protos.Librarian.V1.InternalID { Id = request.Id.Id },
            // Don't need binaries or save files for basic GetStoreApp
            AppBinaryLimit = 0,
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
            
            // Convert Sephirah StoreApp to Angela StoreApp format
            var sephirahStoreApp = sephirahResponse.StoreApp.StoreApp;
            var storeApp = new Librarian.Sephirah.Angela.StoreApp
            {
                Id = new Librarian.Sephirah.Angela.InternalID { Id = sephirahStoreApp.Id.Id },
                Name = sephirahStoreApp.Name,
                Type = sephirahStoreApp.Type.ToString(),
                Description = sephirahStoreApp.Description,
                IconImageId = new Librarian.Sephirah.Angela.InternalID { Id = sephirahStoreApp.IconImageId.Id },
                BackgroundImageId = new Librarian.Sephirah.Angela.InternalID { Id = sephirahStoreApp.BackgroundImageId.Id },
                CoverImageId = new Librarian.Sephirah.Angela.InternalID { Id = sephirahStoreApp.CoverImageId.Id },
                Developer = sephirahStoreApp.Developer,
                Publisher = sephirahStoreApp.Publisher,
                IsPublic = sephirahStoreApp.Public
            };
            storeApp.Tags.AddRange(sephirahStoreApp.Tags);
            storeApp.AltNames.AddRange(sephirahStoreApp.NameAlternatives);

            return new Librarian.Sephirah.Angela.GetStoreAppResponse
            {
                StoreApp = storeApp
            };
        }
        catch (RpcException ex)
        {
            // Re-throw Sephirah errors
            throw new RpcException(new Status(ex.StatusCode, ex.Status.Detail));
        }
    }
}