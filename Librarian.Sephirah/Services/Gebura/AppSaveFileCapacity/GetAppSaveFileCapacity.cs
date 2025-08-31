using Grpc.Core;
using Librarian.Common.Converters;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<GetAppSaveFileCapacityResponse> GetAppSaveFileCapacity(GetAppSaveFileCapacityRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            long internalId;

            if (request.AppId == null || request.AppId.Id == 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppId is required."));
            }

            internalId = request.AppId.Id;

            // get capacity by app
            var appSaveFileCapacity = _dbContext.AppSaveFileCapacities
                .Where(x => x.UserId == userId)
                .Where(x => x.EntityType == EntityType.App)
                .Where(x => x.EntityInternalId == internalId)
                .FirstOrDefault();
            // get capacity by user
            if (appSaveFileCapacity == null)
            {
                appSaveFileCapacity = _dbContext.AppSaveFileCapacities
                    .Where(x => x.UserId == userId)
                    .Where(x => x.EntityType == EntityType.User)
                    .Where(x => x.EntityInternalId == userId)
                    .FirstOrDefault();
            }
            // NOTE: AppSaveFileCapacity for user need to be created in user creation
            if (appSaveFileCapacity == null)
            {
                throw new RpcException(new Status(StatusCode.Internal, "AppSaveFileCapacity not found."));
            }

            return Task.FromResult(new GetAppSaveFileCapacityResponse
            {
                //Count = appSaveFileCapacity.Count,
                //CountMax = appSaveFileCapacity.Count,
                //SizeBytes = appSaveFileCapacity.SizeBytes,
                //SizeBytesMax = appSaveFileCapacity.SizeBytes,
                Strategy = appSaveFileCapacity.Strategy.ToEnumByString<AppSaveFileCapacityStrategy>()
            });
        }
    }
}
