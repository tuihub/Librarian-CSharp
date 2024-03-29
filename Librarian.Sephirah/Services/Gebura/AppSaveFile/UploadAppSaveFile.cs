﻿using Grpc.Core;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<UploadAppSaveFileResponse> UploadAppSaveFile(UploadAppSaveFileRequest request, ServerCallContext context)
        {
            // check capacity
            var userId = context.GetInternalIdFromHeader();
            var appId = request.AppId.Id;
            var user = _dbContext.Users.Single(x => x.Id == userId);
            var app = _dbContext.Apps.Single(x => x.Id == appId);
            var result = AppSaveFileCapacityUtil.CheckCapacity(_dbContext, userId, appId, request.FileMetadata.SizeBytes);
            if (result.IsSuccess == false)
            {
                throw new RpcException(new Status(StatusCode.ResourceExhausted, result.Message ?? "Unknown reason."));
            }
            // create file metadata
            var fileMetadataId = _idGenerator.CreateId();
            var fileMetadata = new Common.Models.Db.FileMetadata(fileMetadataId, request.FileMetadata);
            _dbContext.FileMetadatas.Add(fileMetadata);
            // create app save file
            var appSaveFileId = _idGenerator.CreateId();
            var appSaveFile = new AppSaveFile
            {
                Id = appSaveFileId,
                Status = AppSaveFileStatus.Pending,
                FileMetadata = fileMetadata,
                AppId = appId
            };
            _dbContext.AppSaveFiles.Add(appSaveFile);
            var token = JwtUtil.GenerateUploadToken(fileMetadataId);
            // update user and app capacity
            user.TotalAppSaveFileCount++;
            user.TotalAppSaveFileSizeBytes += fileMetadata.SizeBytes;
            app.TotalAppSaveFileCount++;
            app.TotalAppSaveFileSizeBytes += fileMetadata.SizeBytes;
            _dbContext.SaveChanges();
            return Task.FromResult(new UploadAppSaveFileResponse
            {
                UploadToken = token,
            });
        }
    }
}
