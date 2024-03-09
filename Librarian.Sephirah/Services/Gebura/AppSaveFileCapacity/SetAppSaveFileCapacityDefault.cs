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
        public override Task<SetAppSaveFileCapacityDefaultResponse> SetAppSaveFileCapacityDefault(SetAppSaveFileCapacityDefaultRequest request, ServerCallContext context)
        {
            if (request.Strategy == AppSaveFileCapacityStrategy.Unspecified)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Strategy is required."));
            }
            var userId = context.GetInternalIdFromHeader();
            //EntityType entityType;
            if (request.EntityCase == SetAppSaveFileCapacityDefaultRequest.EntityOneofCase.App && request.App == true)
            {
                //entityType = EntityType.App;
            }
            else
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Entity is not correct."));
            }
            var user = _dbContext.Users.Single(x => x.Id == userId);
            user.AppAppSaveFileCapacityCountDefault = request.Count < 0 ? null : request.Count;
            user.AppAppSaveFileCapacitySizeBytesDefault = request.SizeBytes < 0 ? null : request.SizeBytes;
            user.AppAppSaveFileCapacityStrategyDefault = request.Strategy;
            _dbContext.SaveChanges();
            return Task.FromResult(new SetAppSaveFileCapacityDefaultResponse());
        }
    }
}
