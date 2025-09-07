using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    [Authorize]
    public override Task<DeleteAppRunTimeResponse> DeleteAppRunTime(DeleteAppRunTimeRequest request,
        ServerCallContext context)
    {
        var userId = context.GetInternalIdFromHeader();
        var appRunTimeId = request.Id.Id;

        // Find the AppRunTime record
        var appRunTime = _dbContext.AppRunTimes
            .Include(x => x.App)
            .SingleOrDefault(x => x.Id == appRunTimeId);

        if (appRunTime == null) throw new RpcException(new Status(StatusCode.NotFound, "AppRunTime not found."));

        // Verify user permission
        if (appRunTime.App.UserId != userId)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "No permission to delete this AppRunTime."));

        // Subtract this record's time from the app's total runtime
        appRunTime.App.TotalRunTime -= appRunTime.Duration;

        // Delete the record
        _dbContext.AppRunTimes.Remove(appRunTime);
        _dbContext.SaveChanges();

        return Task.FromResult(new DeleteAppRunTimeResponse());
    }
}