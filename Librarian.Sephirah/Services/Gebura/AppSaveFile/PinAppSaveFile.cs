using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Minio;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<PinAppSaveFileResponse> PinAppSaveFile(PinAppSaveFileRequest request, ServerCallContext context)
        {
            var id = request.Id.Id;
            var appSaveFile = _dbContext.AppSaveFiles
                .Where(x => x.Id == id)
                .Include(x => x.App)
                .Single(x => x.Id == id);
            if (appSaveFile.App.UserId != context.GetInternalIdFromHeader())
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "You do not have permission to pin this file."));
            }
            appSaveFile.IsPinned = true;
            _dbContext.SaveChanges();
            return Task.FromResult(new PinAppSaveFileResponse());
        }
    }
}
