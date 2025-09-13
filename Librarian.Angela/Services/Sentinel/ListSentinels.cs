using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
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

        foreach (var sentinel in sentinels)
        {
            var sentinelPb = new Sentinel
            {
                Id = new InternalID { Id = sentinel.Id },
                Url = sentinel.Url,
                GetTokenUrlPath = sentinel.GetTokenUrlPath,
                DownloadFileUrlPath = sentinel.DownloadFileUrlPath
            };
            sentinelPb.AltUrls.AddRange(sentinel.AltUrls);
            response.Sentinels.Add(sentinelPb);
        }

        return response;
    }
}