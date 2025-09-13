using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    public override async Task<ListStoreAppBinariesResponse> ListStoreAppBinaries(ListStoreAppBinariesRequest request,
        ServerCallContext context)
    {
        var query = _dbContext.StoreAppBinaries
            .Where(x => x.StoreAppId == request.StoreAppId.Id);

        // Apply pagination
        var pageSize = (int)(request.Paging?.PageSize ?? 10);
        var pageNum = (int)(request.Paging?.PageNum ?? 1);
        var skip = (pageNum - 1) * pageSize;

        var totalCount = await query.CountAsync();
        var binaries = await query.Skip(skip).Take(pageSize).ToListAsync();

        var response = new ListStoreAppBinariesResponse
        {
            Paging = new PagingResponse
            {
                TotalSize = totalCount
            }
        };

        foreach (var binary in binaries)
        {
            var binaryPb = new StoreAppBinary
            {
                Id = new InternalID { Id = binary.Id },
                Name = binary.Name,
                SentinelId = new InternalID { Id = binary.SentinelId },
                SentinelGeneratedId = binary.SentinelGeneratedId,
                StoreAppId = new InternalID { Id = binary.StoreAppId }
            };
            response.Binaries.Add(binaryPb);
        }

        return response;
    }
}