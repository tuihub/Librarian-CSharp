using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<DownloadAppSaveFileResponse> DownloadAppSaveFile(DownloadAppSaveFileRequest request, ServerCallContext context)
        {
            var id = request.Id.Id;
            var appSaveFile = _dbContext.AppSaveFiles
                .Where(x => x.Id == id)
                .Include(x => x.App)
                .Single(x => x.Id == id);
            if (appSaveFile.App.UserId != context.GetInternalIdFromHeader())
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "You do not have permission to download this file."));
            }
            var token = JwtUtil.GenerateDownloadToken(appSaveFile.FileMetadataId);
            return Task.FromResult(new DownloadAppSaveFileResponse
            {
                DownloadToken = token
            });
        }
    }
}
