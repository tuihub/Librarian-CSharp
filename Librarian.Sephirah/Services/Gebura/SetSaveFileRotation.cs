using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<SetSaveFileRotationResponse> SetSaveFileRotation(SetSaveFileRotationRequest request, ServerCallContext context)
        {
            var gameSaveFileRotation = _dbContext.GameSaveFileRotations.SingleOrDefault(x => x.EntityInternalId == request.EntityId.Id
                                                                 && x.EntityType == VaildScopeUtil.FromProtoVaildScope(request.ValidScope));
            if (request.Enabled == false)
            {
                if (gameSaveFileRotation != null)
                {
                    _dbContext.GameSaveFileRotations.Remove(gameSaveFileRotation);
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                gameSaveFileRotation ??= new AppSaveFileCapacity();
                gameSaveFileRotation.EntityInternalId = request.EntityId.Id;
                gameSaveFileRotation.EntityType = VaildScopeUtil.FromProtoVaildScope(request.ValidScope);
                gameSaveFileRotation.Count = request.Count;
                _dbContext.GameSaveFileRotations.Update(gameSaveFileRotation);
                _dbContext.SaveChanges();
            }
            return Task.FromResult(new SetSaveFileRotationResponse());
        }
    }
}
