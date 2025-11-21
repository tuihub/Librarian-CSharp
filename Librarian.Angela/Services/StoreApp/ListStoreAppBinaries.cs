using Grpc.Core;
using Librarian.Common.Helpers;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using InternalID = Librarian.Sephirah.Angela.InternalID;
using ListStoreAppBinariesRequest = Librarian.Sephirah.Angela.ListStoreAppBinariesRequest;
using ListStoreAppBinariesResponse = Librarian.Sephirah.Angela.ListStoreAppBinariesResponse;
using StoreAppBinary = Librarian.Sephirah.Angela.StoreAppBinary;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<ListStoreAppBinariesResponse> ListStoreAppBinaries(ListStoreAppBinariesRequest request,
        ServerCallContext context)
    {
        var storeAppId = request.StoreAppId.Id;

        // Verify store app exists
        var storeAppExists = await _dbContext.StoreApps.AnyAsync(x => x.Id == storeAppId);
        if (!storeAppExists)
            throw new RpcException(new Status(StatusCode.NotFound, "Store app not found"));

        var query = _dbContext.StoreAppBinaries.Where(x => x.StoreAppId == storeAppId);

        var totalSize = await query.CountAsync();

        // Apply paging
        if (request.Paging != null)
        {
            var pagingRequest = s_mapper.Map<TuiHub.Protos.Librarian.V1.PagingRequest>(request.Paging);
            query = query.ApplyPagingRequest(pagingRequest);
        }

        var binaries = await query.ToListAsync();

        var response = new ListStoreAppBinariesResponse
        {
            Paging = new PagingResponse
            {
                TotalSize = totalSize
            }
        };

        foreach (var binary in binaries)
        {
            var angelaBinary = new StoreAppBinary
            {
                Id = new InternalID { Id = binary.Id },
                Name = binary.Name,
                SentinelId = new InternalID { Id = binary.SentinelId },
                SentinelGeneratedId = binary.SentinelGeneratedId,
                StoreAppId = request.StoreAppId
            };
            response.Binaries.Add(angelaBinary);
        }

        return response;
    }
}