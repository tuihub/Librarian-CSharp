using Grpc.Core;
using Librarian.Sephirah.Models;
using Librarian.Sephirah.Utils;
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
                                                                 && x.VaildScope == VaildScopeUtil.FromProtoVaildScope(request.VaildScope));
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
                gameSaveFileRotation ??= new GameSaveFileRotation();
                gameSaveFileRotation.EntityInternalId = request.EntityId.Id;
                gameSaveFileRotation.VaildScope = VaildScopeUtil.FromProtoVaildScope(request.VaildScope);
                gameSaveFileRotation.Count = request.Count;
                _dbContext.GameSaveFileRotations.Update(gameSaveFileRotation);
                _dbContext.SaveChanges();
            }
            return Task.FromResult(new SetSaveFileRotationResponse());
        }
    }
}
