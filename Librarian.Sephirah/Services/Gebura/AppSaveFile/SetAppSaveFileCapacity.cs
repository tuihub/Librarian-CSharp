using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<SetAppSaveFileCapacityResponse> SetAppSaveFileCapacity(SetAppSaveFileCapacityRequest request, ServerCallContext context)
        {
            if (request.Strategy == AppSaveFileCapacityStrategy.Unspecified)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Strategy is required."));
            }
            var userId = context.GetInternalIdFromHeader();
            long internalId;
            EntityType entityType;
            if (request.EntityCase == SetAppSaveFileCapacityRequest.EntityOneofCase.User && request.User == true)
            {
                internalId = userId;
                entityType = EntityType.User;
            }
            else if (request.EntityCase == SetAppSaveFileCapacityRequest.EntityOneofCase.AppId && request.AppId.Id != 0)
            {
                var app = _dbContext.Apps.SingleOrDefault(x => x.Id == request.AppId.Id);
                if (app == null)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
                }
                if (app.UserId != userId)
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "App is not owned by user."));
                }
                internalId = request.AppId.Id;
                entityType = EntityType.App;
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Entity is not correct."));
            }
            var appSaveFileCapacity = _dbContext.AppSaveFileCapacities.SingleOrDefault(x =>
                    x.UserId == userId && x.EntityInternalId == internalId && x.EntityType == entityType);
            if (appSaveFileCapacity == null)
            {
                appSaveFileCapacity = new AppSaveFileCapacity(userId, entityType,
                    internalId, request.Count, request.SizeBytes, request.Strategy);
                _dbContext.AppSaveFileCapacities.Add(appSaveFileCapacity);
            }
            else
            {
                appSaveFileCapacity.Update(request.Count, request.SizeBytes, request.Strategy);
            }
            _dbContext.SaveChanges();
            return Task.FromResult(new SetAppSaveFileCapacityResponse());
        }
    }
}
