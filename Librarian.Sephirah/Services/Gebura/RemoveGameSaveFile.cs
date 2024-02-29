using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Minio;
using Minio.DataModel.Args;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override async Task<RemoveGameSaveFileResponse> RemoveGameSaveFile(RemoveGameSaveFileRequest request, ServerCallContext context)
        {
            var id = request.Id.Id;
            var userId = JwtUtil.GetInternalIdFromHeader(context);
            var gameSaveFile = await _dbContext.GameSaveFiles.SingleAsync(x => x.Id == id);
            var fileMetadata = await _dbContext.FileMetadatas.SingleOrDefaultAsync(x => x.Id == id);
            // only remove in minio when status is Stored
            if (gameSaveFile.Status == AppSaveFileStatus.Stored)
            {
                var minioClient = MinioClientUtil.GetMinioClient();
                var rmArgs = new RemoveObjectArgs()
                                 .WithBucket(GlobalContext.SystemConfig.MinioBucket)
                                 .WithObject(id.ToString());
                await minioClient.RemoveObjectAsync(rmArgs);
                var user = _dbContext.Users.Single(x => x.Id == userId);
                user.GameSaveFileUsedCapacityBytes -= fileMetadata?.SizeBytes ?? 0;
            }
            // remove db entries
            _dbContext.GameSaveFiles.Remove(gameSaveFile);
            if(fileMetadata != null) _dbContext.FileMetadatas.Remove(fileMetadata);
            await _dbContext.SaveChangesAsync();
            return new RemoveGameSaveFileResponse();
        }
    }
}
