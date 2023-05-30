using Grpc.Core;
using Librarian.Sephirah.Models;
using Librarian.Sephirah.Utils;
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
        public override Task<UnpinGameSaveFileResponse> UnpinGameSaveFile(UnpinGameSaveFileRequest request, ServerCallContext context)
        {
            var id = request.Id.Id;
            var gameSaveFile = _dbContext.GameSaveFiles.Single(x => x.Id == id);
            gameSaveFile.IsPinned = false;
            _dbContext.SaveChanges();
            return Task.FromResult(new UnpinGameSaveFileResponse());
        }
    }
}
