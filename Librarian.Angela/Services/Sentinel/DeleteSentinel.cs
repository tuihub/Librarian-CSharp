using Grpc.Core;
using Librarian.Common.Utils;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<DeleteSentinelResponse> DeleteSentinel(DeleteSentinelRequest request,
        ServerCallContext context)
    {
        // Verify that the user is an administrator
        UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

        var sentinelId = request.Id.Id;
        var sentinel = await _dbContext.Sentinels
            .Include(s => s.SentinelLibraries)
            .FirstOrDefaultAsync(s => s.Id == sentinelId);

        if (sentinel == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Sentinel does not exist"));

        // Delete the sentinel and its related libraries (cascade should handle this)
        _dbContext.Sentinels.Remove(sentinel);
        await _dbContext.SaveChangesAsync();

        return new DeleteSentinelResponse();
    }
}