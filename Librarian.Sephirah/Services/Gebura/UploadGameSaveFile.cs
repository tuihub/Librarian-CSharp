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
        public override Task<UploadGameSaveFileResponse> UploadGameSaveFile(UploadGameSaveFileRequest request, ServerCallContext context)
        {
            // check rotation count
            var userInternalId = JwtUtil.GetInternalIdFromJwt(context);
            var appPackageInternalId = request.AppPackageId.Id;
            var appPackageSaveFileCount = _dbContext.GameSaveFiles.Count(x => x.AppPackageId == appPackageInternalId);
            var saveFileRotationCount = GameSaveFileRotationUtil.GetGameSaveFileRotation(_dbContext, userInternalId, appPackageInternalId);
            if (appPackageSaveFileCount >= saveFileRotationCount)
                throw new RpcException(new Status(StatusCode.ResourceExhausted,
                    $"Rotation limit reached or exceeded({appPackageSaveFileCount} used / {saveFileRotationCount} limit)."));
            // check capacity
            var user = _dbContext.Users.Single(x => x.Id == userInternalId);
            if (user.GameSaveFileCapacityBytes != null && user.GameSaveFileUsedCapacityBytes + request.FileMetadata.SizeBytes > user.GameSaveFileCapacityBytes)
                throw new RpcException(new Status(StatusCode.ResourceExhausted,
                        $"User game save file capacity exceeded({HumanizeUtil.BytesToString(user.GameSaveFileUsedCapacityBytes)} used" +
                        $" + {HumanizeUtil.BytesToString(request.FileMetadata.SizeBytes)} file" +
                        $" > {HumanizeUtil.BytesToString((long)user.GameSaveFileCapacityBytes)} limit)."));
            var internalId = IdUtil.NewId();
            var fileMetadata = new Common.Models.FileMetadata(internalId, request.FileMetadata);
            var gameSaveFile = new GameSaveFile
            {
                Id = internalId,
                Status = GameSaveFileStatus.Pending,
                FileMetadata = fileMetadata,
                UserId = userInternalId,
                AppPackageId = appPackageInternalId,
            };
            _dbContext.GameSaveFiles.Add(gameSaveFile);
            var token = JwtUtil.GenerateUploadToken(internalId);
            // update user save file capacity stat
            user.GameSaveFileUsedCapacityBytes += fileMetadata.SizeBytes;
            _dbContext.SaveChanges();
            return Task.FromResult(new UploadGameSaveFileResponse
            {
                UploadToken = token,
            });
        }
    }
}
