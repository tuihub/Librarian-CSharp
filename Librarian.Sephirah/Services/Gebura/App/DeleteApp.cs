using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;
using App = Librarian.Common.Models.Db.App;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    [Authorize]
    public override Task<DeleteAppResponse> DeleteApp(DeleteAppRequest request, ServerCallContext context)
    {
        // create app
        var userId = context.GetInternalIdFromHeader();
        var appId = request.Id.Id;

        var app = _dbContext.Apps.SingleOrDefault(x => x.Id == appId && x.UserId == userId);
        
        if (app == null)
            throw new RpcException(new Status(StatusCode.NotFound, "App not exists."));
        
        _dbContext.Apps.Remove(app);

        _dbContext.SaveChanges();
        return Task.FromResult(new DeleteAppResponse());
    }
}