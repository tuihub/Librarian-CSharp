using Google.Protobuf;
using Grpc.Core;
using Librarian.Sephirah.Models;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using Minio;
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
            using var db = new ApplicationDbContext();
            var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
            var internalId = JwtUtil.GetInternalIdFromToken(token);
            var gameSaveFile = db.GameSaveFiles.Single(x => x.Id == internalId);
            if (gameSaveFile.Status != GameSaveFileStatus.STORED)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Requested game save is not stored."));
            var fileMetadata = db.FileMetadatas.Single(x => x.Id == internalId);
            // pipe stream
            var pipeStreamServer = new AnonymousPipeServerStream();
            var pipeStreamClient = new AnonymousPipeClientStream(pipeStreamServer.GetClientHandleAsString());
            // get object from minio
            var minioClient = new MinioClient()
                                  .WithEndpoint(GlobalContext.SystemConfig.MinioEndpoint)
                                  .WithCredentials(
                                      GlobalContext.SystemConfig.MinioAccessKey,
                                      GlobalContext.SystemConfig.MinioSecretKey)
                                  .WithSSL(GlobalContext.SystemConfig.MinioWithSSL)
                                  .Build();
            var getObjectArgs = new GetObjectArgs()
                                    .WithBucket(GlobalContext.SystemConfig.MinioBucket)
                                    .WithObject(internalId.ToString())
                                    .WithCallbackStream(async stream =>
                                    {
                                        var buffer = new byte[GlobalContext.SystemConfig.BinahChunkBytes];
                                        while (true)
                                        {
                                            var bytesRead = await stream.ReadAsync(buffer);
                                            if (bytesRead == 0)
                                            {
                                                break;
                                            }
                                            await pipeStreamServer.WriteAsync(buffer);
                                        }
                                    });
            // send
            var buffer = new byte[GlobalContext.SystemConfig.BinahChunkBytes];
            while (true)
            {
                var numBytesRead = await pipeStreamClient.ReadAsync(buffer);
                if (numBytesRead == 0)
                {
                    break;
                }
                await responseStream.WriteAsync(new SimpleDownloadFileResponse
                {
                    Data = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, numBytesRead))
                });
            }
        }
    }
}
