using Grpc.Core;
using Librarian.Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService
    {
        /// <summary>
        /// Search store apps with optional name filter
        /// </summary>
        [Authorize]
        public override async Task<SearchStoreAppsResponse> SearchStoreApps(SearchStoreAppsRequest request, ServerCallContext context)
        {
            var query = _dbContext.StoreApps.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.NameLike))
            {
                query = query.Where(x => x.Name.Contains(request.NameLike));
            }

            // Apply pagination
            query = query.ApplyPagingRequest(request.Paging);

            // Execute query
            var storeApps = await query.ToListAsync();

            // Map to response
            var response = new SearchStoreAppsResponse
            {
                Paging = new PagingResponse
                {
                    TotalSize = storeApps.Count,
                }
            };

            // Map store apps to StoreAppDigest objects
            foreach (var storeApp in storeApps)
            {
                var storeAppDigest = new StoreAppDigest
                {
                    Id = new InternalID { Id = storeApp.Id },
                    Name = storeApp.Name,
                    Type = Common.Converters.EnumConverter.ToEnumByString<AppType>(storeApp.Type),
                    ShortDescription = storeApp.Description.Length > 100 ? storeApp.Description[..100] : storeApp.Description,
                    CoverImageId = new InternalID { Id = storeApp.CoverImageId }
                };

                // Add tags
                storeAppDigest.Tags.AddRange(storeApp.Tags);

                response.AppInfos.Add(storeAppDigest);
            }

            return response;
        }
    }
}