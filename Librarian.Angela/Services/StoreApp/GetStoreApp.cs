using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    public override async Task<GetStoreAppResponse> GetStoreApp(GetStoreAppRequest request,
        ServerCallContext context)
    {
        var storeApp = await _dbContext.StoreApps
            .FirstOrDefaultAsync(x => x.Id == request.Id.Id);

        if (storeApp == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Store app not found"));

        var storeAppPb = new StoreApp
        {
            Id = new InternalID { Id = storeApp.Id },
            Name = storeApp.Name,
            Type = storeApp.Type.ToString(),
            Description = storeApp.Description,
            IconImageId = new InternalID { Id = storeApp.IconImageId },
            BackgroundImageId = new InternalID { Id = storeApp.BackgroundImageId },
            CoverImageId = new InternalID { Id = storeApp.CoverImageId },
            Developer = storeApp.Developer,
            Publisher = storeApp.Publisher,
            IsPublic = storeApp.IsPublic
        };
        storeAppPb.Tags.AddRange(storeApp.Tags);
        storeAppPb.AltNames.AddRange(storeApp.AltNames);

        return new GetStoreAppResponse
        {
            StoreApp = storeAppPb
        };
    }
}