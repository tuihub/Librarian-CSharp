using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    [Authorize]
    public override Task<UpdateAppResponse> UpdateApp(UpdateAppRequest request, ServerCallContext context)
    {
        var appReq = request.App;
        var app = _dbContext.Apps.SingleOrDefault(x => x.Id == appReq.Id.Id);
        if (app == null) 
            throw new RpcException(new Status(StatusCode.NotFound, "App not exists."));
        
        // Check ownership
        var userId = context.GetInternalIdFromHeader();
        if (app.UserId != userId)
            throw new RpcException(new Status(StatusCode.PermissionDenied, 
                "You do not have permission to update this app."));
        
        // Check if app is managed by store
        if (app.BoundStoreAppId != 0 && !app.StopStoreManage)
            throw new RpcException(new Status(StatusCode.FailedPrecondition, 
                "This app is managed by store and cannot be updated. Set stop_store_manage to true first."));
        
        // Validate StopStoreManage can only be set when bound to store
        if (appReq.HasStopStoreManage && app.BoundStoreAppId == 0)
            throw new RpcException(new Status(StatusCode.InvalidArgument, 
                "Cannot set stop_store_manage on apps not bound to store."));
        
        // Update app (without server-controlled fields)
        app.UpdateFromPb(appReq, updateServerControlledFields: false);
        _dbContext.SaveChanges();
        return Task.FromResult(new UpdateAppResponse());
    }
}