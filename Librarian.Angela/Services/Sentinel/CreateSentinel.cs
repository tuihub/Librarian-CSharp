using Grpc.Core;
using Librarian.Common.Utils;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<CreateSentinelResponse> CreateSentinel(CreateSentinelRequest request,
        ServerCallContext context)
    {
        // Verify that the user is an administrator
        UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

        // Generate a new internal ID using a simple timestamp approach
        var internalId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Create a new sentinel
        var sentinel = new Common.Models.Db.Sentinel
        {
            Id = internalId,
            UserId = request.Sentinel.UserId.Id,
            Url = request.Sentinel.Url,
            AltUrls = request.Sentinel.AltUrls.ToList(),
            GetTokenUrlPath = request.Sentinel.GetTokenUrlPath,
            DownloadFileUrlPath = request.Sentinel.DownloadFileUrlPath,
            RefreshToken = request.Sentinel.RefreshToken ?? string.Empty
        };

        _dbContext.Sentinels.Add(sentinel);
        await _dbContext.SaveChangesAsync();

        return new CreateSentinelResponse
        {
            Id = new InternalID { Id = internalId }
        };
    }
}
