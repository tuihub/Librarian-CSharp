using Google.Protobuf;
using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Minio;
using Minio.DataModel.Args;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Cryptography;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // UNDONE: Current only support GameSaveFile download
        [Authorize(AuthenticationSchemes = "DownloadToken")]
        public override async Task SimpleDownloadFile(SimpleDownloadFileRequest request, IServerStreamWriter<SimpleDownloadFileResponse> responseStream, ServerCallContext context)
        {
            var internalId = context.GetInternalIdFromHeader();
            var gameSaveFile = _dbContext.GameSaveFiles.Single(x => x.Id == internalId);
            if (gameSaveFile.Status != AppSaveFileStatus.Stored)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Requested game save is not stored."));
            var fileMetadata = _dbContext.FileMetadatas.Single(x => x.Id == internalId);
            // get object from minio
            var minioClient = MinioClientUtil.GetMinioClient();
            var getObjectArgs = new GetObjectArgs()
                                    .WithBucket(GlobalContext.SystemConfig.MinioBucket)
                                    .WithObject(internalId.ToString())
                                    .WithCallbackStream(stream =>
                                    {
                                        Task.Run(async () =>
                                        {
                                            var buffer = new byte[GlobalContext.SystemConfig.BinahChunkBytes];
                                            while (true)
                                            {
                                                var bytesRead = await stream.ReadAsync(buffer);
                                                Debug.WriteLine($"SimpleDownloadFile: callback stream bytesRead = {bytesRead}");
                                                if (bytesRead == 0)
                                                {
                                                    break;
                                                }
                                                await responseStream.WriteAsync(new SimpleDownloadFileResponse
                                                {
                                                    Data = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, bytesRead))
                                                });
                                                Debug.WriteLine($"SimpleDownloadFile: sent {bytesRead} bytes");
                                            }
                                        }).Wait();
                                    });
            await minioClient.GetObjectAsync(getObjectArgs);
            Debug.WriteLine($"SimpleDownloadFile: GetObject finished");
        }
    }
}
