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
        public override Task<GetAppSaveFileCapacityResponse> GetAppSaveFileCapacity(GetAppSaveFileCapacityRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            long internalId;
            EntityType entityType;
            if (request.EntityCase == GetAppSaveFileCapacityRequest.EntityOneofCase.User && request.User == true)
            {
                internalId = userId;
                entityType = EntityType.User;
            }
            else if (request.EntityCase == GetAppSaveFileCapacityRequest.EntityOneofCase.AppId && request.AppId.Id != 0)
            {
                entityType = EntityType.App;
                internalId = request.AppId.Id;
            }
            else if (request.EntityCase == GetAppSaveFileCapacityRequest.EntityOneofCase.AppInstId && request.AppInstId.Id != 0)
            {
                entityType = EntityType.AppInst;
                internalId = request.AppInstId.Id;
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Entity is not correct."));
            }
            var appSaveFileCapacity = AppSaveFileCapacityUtil.GetAppSaveFileCapacity(_dbContext, userId, entityType, internalId);
            return Task.FromResult(new GetAppSaveFileCapacityResponse
            {
                Count = appSaveFileCapacity.Count ?? -1,
                SizeBytes = appSaveFileCapacity.SizeBytes ?? -1,
                Strategy = appSaveFileCapacity.Strategy
            });
        }
    }
}
