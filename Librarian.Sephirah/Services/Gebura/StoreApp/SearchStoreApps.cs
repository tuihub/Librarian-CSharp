using Grpc.Core;
using Librarian.Common.Converters;
using Librarian.Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService
{
    /// <summary>
    ///     Search store apps with optional name filter
    ///     By default, only returns public store apps
    /// </summary>
    [Authorize]
    public override async Task<SearchStoreAppsResponse> SearchStoreApps(SearchStoreAppsRequest request,
        ServerCallContext context)
    {
        // Default to showing only public store apps
        var query = _dbContext.StoreApps.Where(x => x.IsPublic);

        if (!string.IsNullOrWhiteSpace(request.NameLike)) 
            query = query.Where(x => x.Name.Contains(request.NameLike));

        // Total count before pagination
        var totalSize = await query.CountAsync();

        // Apply pagination and execute
        query = query.ApplyPagingRequest(request.Paging);
        var storeApps = await query.ToListAsync();

        // Map to response
        var response = new SearchStoreAppsResponse
        {
            Paging = new PagingResponse
            {
                TotalSize = totalSize
            }
        };

        // Map store apps to StoreAppDigest objects
        foreach (var storeApp in storeApps)
        {
            var storeAppDigest = new StoreAppDigest
            {
                Id = new InternalID { Id = storeApp.Id },
                Name = storeApp.Name,
                Type = EnumConverter.ToEnumByString<AppType>(storeApp.Type),
                ShortDescription = storeApp.Description.Length > 100
                    ? storeApp.Description[..100]
                    : storeApp.Description,
                CoverImageId = new InternalID { Id = storeApp.CoverImageId }
            };

            // Add tags
            storeAppDigest.Tags.AddRange(storeApp.Tags);

            response.AppInfos.Add(storeAppDigest);
        }

        return response;
    }
}