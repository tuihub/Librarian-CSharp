using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    public override async Task<SearchStoreAppsResponse> SearchStoreApps(SearchStoreAppsRequest request,
        ServerCallContext context)
    {
        var query = _dbContext.StoreApps.AsQueryable();

        // Apply name filter
        if (!string.IsNullOrWhiteSpace(request.NameLike))
            query = query.Where(x => x.Name.Contains(request.NameLike));

        // Apply pagination
        var pageSize = (int)(request.Paging?.PageSize ?? 10);
        var pageNum = (int)(request.Paging?.PageNum ?? 1);
        var skip = (pageNum - 1) * pageSize;

        var totalCount = await query.CountAsync();
        var storeApps = await query.Skip(skip).Take(pageSize).ToListAsync();

        var response = new SearchStoreAppsResponse
        {
            Paging = new PagingResponse
            {
                TotalSize = totalCount
            }
        };

        foreach (var storeApp in storeApps)
        {
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
            response.StoreApps.Add(storeAppPb);
        }

        return response;
    }
}