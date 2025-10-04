using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<ListSentinelsResponse> ListSentinels(ListSentinelsRequest request,
        ServerCallContext context)
    {
        var query = _dbContext.Sentinels.AsQueryable();

        // Apply pagination
        var pageSize = (int)(request.Paging?.PageSize ?? 10);
        var pageNum = (int)(request.Paging?.PageNum ?? 1);
        var skip = (pageNum - 1) * pageSize;

        var totalCount = await query.CountAsync();
        var sentinels = await query.Skip(skip).Take(pageSize).ToListAsync();

        var response = new ListSentinelsResponse
        {
            Paging = new PagingResponse
            {
                TotalSize = totalCount
            }
        };

        // Use AutoMapper to convert entities to protobuf objects
        foreach (var sentinel in sentinels)
        {
            var sentinelPb = _mapper.Map<Sentinel>(sentinel);
            response.Sentinels.Add(sentinelPb);
        }

        return response;
    }
}