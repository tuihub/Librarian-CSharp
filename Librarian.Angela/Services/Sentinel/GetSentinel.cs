using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize]
    public override async Task<GetSentinelResponse> GetSentinel(GetSentinelRequest request,
        ServerCallContext context)
    {
        var sentinel = await _dbContext.Sentinels
            .FirstOrDefaultAsync(x => x.Id == request.Id.Id);

        if (sentinel == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Sentinel not found"));

        var sentinelPb = new Sentinel
        {
            Id = new InternalID { Id = sentinel.Id },
            Url = sentinel.Url,
            GetTokenUrlPath = sentinel.GetTokenUrlPath,
            DownloadFileUrlPath = sentinel.DownloadFileUrlPath
        };
        sentinelPb.AltUrls.AddRange(sentinel.AltUrls);

        return new GetSentinelResponse
        {
            Sentinel = sentinelPb
        };
    }
}