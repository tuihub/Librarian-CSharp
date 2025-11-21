using Grpc.Core;
using Librarian.Common.Helpers;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InternalID = Librarian.Sephirah.Angela.InternalID;
using PagingResponse = Librarian.Sephirah.Angela.PagingResponse;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
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
        if (request.Paging != null)
        {
            var pagingRequest = s_mapper.Map<TuiHub.Protos.Librarian.V1.PagingRequest>(request.Paging);
            query = query.ApplyPagingRequest(pagingRequest);
        }
        var storeApps = await query.ToListAsync();

        var response = new SearchStoreAppsResponse
        {
            Paging = new PagingResponse
            {
                TotalSize = totalSize
            }
        };

        foreach (var storeApp in storeApps)
        {
            var angelaStoreApp = new StoreApp
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
            };
            angelaStoreApp.Tags.AddRange(storeApp.Tags);
            response.StoreApps.Add(angelaStoreApp);
        }

        return response;
    }
}