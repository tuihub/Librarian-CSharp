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
        public override Task<UploadGameSaveFileResponse> UploadGameSaveFile(UploadGameSaveFileRequest request, ServerCallContext context)
        {
            var userInternalId = JwtUtil.GetInternalIdFromJwt(context);
            var appInternalId = request.AppPackageId.Id;
            var internalId = IdUtil.NewId();
            var fileMetadata = new Models.FileMetadata(internalId, request.FileMetadata);
            var gameSaveFile = new GameSaveFile
            {
                Id = internalId,
                Status = GameSaveFileStatus.Pending,
                FileMetadata = fileMetadata,
                UserId = userInternalId,
                AppPackageId = appInternalId,
            };
            _dbContext.GameSaveFiles.Add(gameSaveFile);
            var token = JwtUtil.GenerateUploadToken(internalId);
            _dbContext.SaveChanges();
            return Task.FromResult(new UploadGameSaveFileResponse
            {
                UploadToken = token,
            });
        }
    }
}
