using Google.Protobuf;
using Grpc.Core;
using Librarian.Sephirah.Models;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using Minio;
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
            var internalId = JwtUtil.GetInternalIdFromJwt(context);
            var gameSaveFile = _dbContext.GameSaveFiles.Single(x => x.Id == internalId);
            if (gameSaveFile.Status != GameSaveFileStatus.Stored)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Requested game save is not stored."));
            var fileMetadata = _dbContext.FileMetadatas.Single(x => x.Id == internalId);
            // get object stat from minio 
            //var statObjectArgs = new StatObjectArgs()
            //                         .WithBucket(GlobalContext.SystemConfig.MinioBucket)
            //                         .WithObject(internalId.ToString());
            //var objectStat = await minioClient.StatObjectAsync(statObjectArgs);
            // get object from minio
            var minioClient = MinioClientUtil.GetMinioClient();
            var getObjectArgs = new GetObjectArgs()
                                    .WithBucket(GlobalContext.SystemConfig.MinioBucket)
                                    .WithObject(internalId.ToString())
                                    .WithCallbackStream(stream =>
                                    {
                                        var ms = new MemoryStream();
                                        stream.CopyTo(ms);
                                        Debug.WriteLine($"SimpleDownloadFile: ms.Length = {ms.Length}");
                                        ms.Position = 0;
                                        var buffer = new byte[GlobalContext.SystemConfig.BinahChunkBytes];
                                        while (true)
                                        {
                                            var bytesRead = ms.Read(buffer);
                                            Debug.WriteLine($"SimpleDownloadFile: callback stream bytesRead = {bytesRead}");
                                            if (bytesRead == 0)
                                            {
                                                break;
                                            }
                                            // must wait?
                                            responseStream.WriteAsync(new SimpleDownloadFileResponse
                                            {
                                                Data = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, bytesRead))
                                            }).Wait();
                                            Debug.WriteLine($"SimpleDownloadFile: sent {bytesRead} bytes");
                                        }
                                    });
            await minioClient.GetObjectAsync(getObjectArgs);
            Debug.WriteLine($"SimpleDownloadFile: GetObject finished");
        }
    }
}
