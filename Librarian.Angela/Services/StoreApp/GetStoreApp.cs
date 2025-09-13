using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    public override async Task<Librarian.Sephirah.Angela.GetStoreAppResponse> GetStoreApp(Librarian.Sephirah.Angela.GetStoreAppRequest request,
        ServerCallContext context)
    {
        // Convert Angela request to Sephirah request
        var sephirahRequest = new GetStoreAppSummaryRequest
        {
            StoreAppId = new TuiHub.Protos.Librarian.V1.InternalID { Id = request.Id.Id },
            AppBinaryLimit = 0, // Don't need binaries for basic info
            AppSaveFileLimit = 0, // Don't need save files for basic info
            AcquiredUserLimit = 0 // Don't need users for basic info
        };

        // Forward authorization header
        var headers = new Metadata();
        if (context.RequestHeaders.Any(h => h.Key == "authorization"))
        {
            var authHeader = context.RequestHeaders.First(h => h.Key == "authorization");
            headers.Add(authHeader);
        }

        try
        {
            // Call Sephirah service
            var sephirahResponse = await _sephirahClient.GetStoreAppSummaryAsync(sephirahRequest, headers);

            var sephirahStoreApp = sephirahResponse.StoreApp.StoreApp;

            // Convert Sephirah response to Angela response
            var angelaStoreApp = new Librarian.Sephirah.Angela.StoreApp
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
            angelaStoreApp.Tags.AddRange(sephirahStoreApp.Tags);
            angelaStoreApp.AltNames.AddRange(sephirahStoreApp.NameAlternatives);

            return new Librarian.Sephirah.Angela.GetStoreAppResponse
            {
                StoreApp = angelaStoreApp
            };
        }
        catch (RpcException)
        {
            // Re-throw RPC exceptions (like NotFound) as-is
            throw;
        }
    }
}