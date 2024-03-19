using Grpc.Core;
using Librarian.Common;
using Librarian.Common.Models.Db;
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
        public override async Task<RemoveAppSaveFileResponse> RemoveAppSaveFile(RemoveAppSaveFileRequest request, ServerCallContext context)
        {
            var id = request.Id.Id;
            var userId = context.GetInternalIdFromHeader();
            var appSaveFile = await _dbContext.AppSaveFiles
                .Where(x => x.Id == id)
                .Include(x => x.App)
                .SingleAsync(x => x.Id == id);
            if (appSaveFile.App.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "You do not have permission to remove this file."));
            }
            var fileMetadata = await _dbContext.FileMetadatas.SingleAsync(x => x.Id == appSaveFile.FileMetadataId);
            // only remove in minio when status is Stored
            if (appSaveFile.Status == AppSaveFileStatus.Stored)
            {
                var minioClient = MinioClientUtil.GetMinioClient();
                var rmArgs = new RemoveObjectArgs()
                                 .WithBucket(GlobalContext.SystemConfig.MinioBucket)
                                 .WithObject(appSaveFile.FileMetadataId.ToString());
                await minioClient.RemoveObjectAsync(rmArgs);
            }
            // update user and app capacity
            var user = _dbContext.Users.Single(x => x.Id == userId);
            user.TotalAppSaveFileCount--;
            user.TotalAppSaveFileSizeBytes -= fileMetadata.SizeBytes;
            appSaveFile.App.TotalAppSaveFileCount--;
            appSaveFile.App.TotalAppSaveFileSizeBytes -= fileMetadata.SizeBytes;
            // remove db entries
            _dbContext.FileMetadatas.Remove(fileMetadata);
            _dbContext.AppSaveFiles.Remove(appSaveFile);
            await _dbContext.SaveChangesAsync();
            return new RemoveAppSaveFileResponse();
        }
    }
}
