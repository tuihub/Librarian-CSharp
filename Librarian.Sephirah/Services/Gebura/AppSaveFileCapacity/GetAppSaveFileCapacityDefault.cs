using Grpc.Core;
using Librarian.Common.Models.Db;
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
        public override Task<GetAppSaveFileCapacityDefaultResponse> GetAppSaveFileCapacityDefault(GetAppSaveFileCapacityDefaultRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            //EntityType entityType;
            if (request.EntityCase == GetAppSaveFileCapacityDefaultRequest.EntityOneofCase.App && request.App == true)
            {
                //entityType = EntityType.App;
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Entity is not correct."));
            }
            var user = _dbContext.Users.Single(x => x.Id == userId);
            return Task.FromResult(new GetAppSaveFileCapacityDefaultResponse
            {
                Count = user.AppAppSaveFileCapacityCountDefault ?? -1,
                SizeBytes = user.AppAppSaveFileCapacitySizeBytesDefault ?? -1,
                Strategy = user.AppAppSaveFileCapacityStrategyDefault
            });
        }
    }
}
