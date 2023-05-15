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
        // UNDONE: Current only support GameSaveFile upload
        [Authorize(AuthenticationSchemes = "UploadToken")]
        public override async Task SimpleUploadFile(IAsyncStreamReader<SimpleUploadFileRequest> requestStream, IServerStreamWriter<SimpleUploadFileResponse> responseStream, ServerCallContext context)
        {
            using var db = new ApplicationDbContext();
            var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
            var internalId = JwtUtil.GetInternalIdFromToken(token);
            var gameSaveFile = db.GameSaveFiles.Single(x => x.Id == internalId);
            if (gameSaveFile.Status == GameSaveFileStatus.STORED)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Current game save has been stored."));
            var fileMetadata = db.FileMetadatas.Single(x => x.Id == internalId);
            // two stream, one for Sha256, the other for Minio
            var pipeStreamServer4Sha256 = new AnonymousPipeServerStream();
            var pipeStreamClient4Sha256 = new AnonymousPipeClientStream(pipeStreamServer4Sha256.GetClientHandleAsString());
            var pipeStreamServer4Minio = new AnonymousPipeServerStream();
            var pipeStreamClient4Minio = new AnonymousPipeClientStream(pipeStreamServer4Minio.GetClientHandleAsString());
            //using var memoryStream = new MemoryStream();
            long receivedBytes = 0;
            var readTask = Task.Run(async () =>
            {
                gameSaveFile.Status = GameSaveFileStatus.IN_PROGRESS;
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    if (message.Data != null)
                    {
                        //await memoryStream.WriteAsync(message.Data.Memory);
                        await pipeStreamServer4Sha256.WriteAsync(message.Data.Memory);
                        await pipeStreamServer4Minio.WriteAsync(message.Data.Memory);
                        receivedBytes += message.Data.Length;
                    }
                }
            });
            var writeTaskCancellationTokenSource = new CancellationTokenSource();
            var writeTask = Task.Run(async () =>
            {
                long executionNum = 0;
                while (readTask.IsCompleted == false)
                {
                    if (writeTaskCancellationTokenSource.IsCancellationRequested == true)
                    {
                        break;
                    }
                    Thread.Sleep(200);
                    if (executionNum % 15 == 0)
                    {
                        await responseStream.WriteAsync(new SimpleUploadFileResponse { Status = FileTransferStatus.InProgress });
                    }
                    executionNum++;
                }
            }, writeTaskCancellationTokenSource.Token);
            // upload to minio
            var minioClient = new MinioClient()
                                  .WithEndpoint(GlobalContext.SystemConfig.MinioEndpoint)
                                  .WithCredentials(
                                      GlobalContext.SystemConfig.MinioAccessKey,
                                      GlobalContext.SystemConfig.MinioSecretKey)
                                  .WithSSL(GlobalContext.SystemConfig.MinioWithSSL)
                                  .Build();
            var putObjectArgs = new PutObjectArgs()
                                    .WithBucket(GlobalContext.SystemConfig.MinioBucket)
                                    .WithObject(internalId.ToString())
                                    .WithStreamData(pipeStreamClient4Minio);
            var minioPutTask = minioClient.PutObjectAsync(putObjectArgs);
            // calc Sha256
            using var sha256 = SHA256.Create();
            var sha256Task = sha256.ComputeHashAsync(pipeStreamClient4Sha256);
            // wait for readTask, minioPutTask, sha256Task
            Task.WaitAll(readTask, minioPutTask, sha256Task);
            // compare sha256 digest in db
            if (sha256Task.Result.SequenceEqual(fileMetadata.Sha256.ToArray()) == false)
            {
                // remove object in Minio
                var removeObjectArgs = new RemoveObjectArgs()
                                           .WithBucket(GlobalContext.SystemConfig.MinioBucket)
                                           .WithObject(internalId.ToString());
                await minioClient.RemoveObjectAsync(removeObjectArgs);
                // update db
                gameSaveFile.Status = GameSaveFileStatus.SHA256_MISMATCH;
                // throw rpcex
                throw new RpcException(new Status(StatusCode.InvalidArgument, "SHA256 digest not match."));
            }
            // cancel writeTask(cdn keep alive)
            writeTaskCancellationTokenSource.Cancel();
            // update db
            var ret = new SimpleUploadFileResponse();
            if (gameSaveFile.Status == GameSaveFileStatus.IN_PROGRESS)
            {
                gameSaveFile.Status = GameSaveFileStatus.STORED;
                ret.Status = FileTransferStatus.Success;
            }
            else
            {
                ret.Status = FileTransferStatus.Failed;
            }
            gameSaveFile.UpdatedAt = DateTime.Now;
            db.SaveChanges();
            await responseStream.WriteAsync(ret);
        }
    }
}
