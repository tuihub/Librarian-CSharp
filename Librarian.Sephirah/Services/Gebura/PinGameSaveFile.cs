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
        public override Task<PinGameSaveFileResponse> PinGameSaveFile(PinGameSaveFileRequest request, ServerCallContext context)
        {
            var id = request.Id.Id;
            var gameSaveFile = _dbContext.GameSaveFiles.Single(x => x.Id == id);
            gameSaveFile.IsPinned = true;
            _dbContext.SaveChanges();
            return Task.FromResult(new PinGameSaveFileResponse());
        }
    }
}
