using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<SetUserSaveFileCapacityResponse> SetUserSaveFileCapacity(SetUserSaveFileCapacityRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            // set capacity
            var userId = request.UserId.Id;
            long? capacityBytes = request.HasCapacityBytes ? request.CapacityBytes : null;
            var user = _dbContext.Users.Single(x => x.Id == userId);
            user.GameSaveFileCapacityBytes = capacityBytes;
            _dbContext.SaveChanges();
            return Task.FromResult(new SetUserSaveFileCapacityResponse());
        }
    }
}
