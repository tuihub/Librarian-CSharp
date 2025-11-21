using Grpc.Core;
using Librarian.Common.Utils;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<UpdateSentinelResponse> UpdateSentinel(UpdateSentinelRequest request,
        ServerCallContext context)
    {
        var sentinelId = request.Sentinel.Id.Id;
        var sentinel = await _dbContext.Sentinels.FirstOrDefaultAsync(s => s.Id == sentinelId);

        if (sentinel == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Sentinel does not exist"));

        // Update sentinel information
        if (request.Sentinel.UserId != null && request.Sentinel.UserId.Id != 0)
            sentinel.UserId = request.Sentinel.UserId.Id;

        if (!string.IsNullOrEmpty(request.Sentinel.Url))
            sentinel.Url = request.Sentinel.Url;

        if (request.Sentinel.AltUrls.Count > 0)
            sentinel.AltUrls = request.Sentinel.AltUrls.ToList();

        if (!string.IsNullOrEmpty(request.Sentinel.GetTokenUrlPath))
            sentinel.GetTokenUrlPath = request.Sentinel.GetTokenUrlPath;

        if (!string.IsNullOrEmpty(request.Sentinel.DownloadFileUrlPath))
            sentinel.DownloadFileUrlPath = request.Sentinel.DownloadFileUrlPath;

        if (!string.IsNullOrEmpty(request.Sentinel.RefreshToken))
            sentinel.RefreshToken = request.Sentinel.RefreshToken;

        sentinel.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return new UpdateSentinelResponse();
    }
}