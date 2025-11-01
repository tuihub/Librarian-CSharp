using Grpc.Core;
using Librarian.Common.Constants;
using Librarian.Common.Converters;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    [Authorize]
    public override Task<SetAppSaveFileCapacityResponse> SetAppSaveFileCapacity(SetAppSaveFileCapacityRequest request,
        ServerCallContext context)
    {
        if (request.Strategy == AppSaveFileCapacityStrategy.Unspecified)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Strategy is required."));
        var userId = context.GetInternalIdFromHeader();
        long internalId;

        if (!request.ApplyToAll && (request.AppId == null || request.AppId.Id == 0))
            throw new RpcException(new Status(StatusCode.InvalidArgument, "ApplyToAll or AppId is required."));

        var app = _dbContext.Apps.SingleOrDefault(x => x.Id == request.AppId.Id);
        if (app == null) throw new RpcException(new Status(StatusCode.NotFound, "App not exists."));
        if (app.UserId != userId)
            throw new RpcException(new Status(StatusCode.PermissionDenied, "App is not owned by user."));
        internalId = request.AppId.Id;

        var strategy = request.Strategy.ToEnumByString<Enums.AppSaveFileCapacityStrategy>();

        // user level capacity
        if (request.ApplyToAll)
            UpdateAppSaveFileCapacity(
                userId,
                EntityType.User,
                userId,
                request.Count,
                request.SizeBytes,
                strategy);
        // app level capacity
        else
            UpdateAppSaveFileCapacity(
                userId,
                EntityType.App,
                internalId,
                request.Count,
                request.SizeBytes,
                strategy);

        _dbContext.SaveChanges();
        return Task.FromResult(new SetAppSaveFileCapacityResponse());
    }

    private void UpdateAppSaveFileCapacity(
        long userId,
        EntityType entityType,
        long internalId,
        long count,
        long sizeBytes,
        Enums.AppSaveFileCapacityStrategy strategy)
    {
        var appSaveFileCapacity = _dbContext.AppSaveFileCapacities.SingleOrDefault(x =>
            x.UserId == userId && x.EntityInternalId == internalId && x.EntityType == entityType);

        if (appSaveFileCapacity == null)
        {
            appSaveFileCapacity = new AppSaveFileCapacity(userId, entityType,
                internalId, count, sizeBytes, strategy);
            _dbContext.AppSaveFileCapacities.Add(appSaveFileCapacity);
        }
        else
        {
            appSaveFileCapacity.Update(count, sizeBytes, strategy);
        }
    }
}