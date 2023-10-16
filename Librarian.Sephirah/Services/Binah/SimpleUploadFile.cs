using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
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
        // UNDONE: Current only support GameSaveFile upload
        // TODO: add check uploaded file
        [Authorize(AuthenticationSchemes = "UploadToken")]
        public override async Task SimpleUploadFile(IAsyncStreamReader<SimpleUploadFileRequest> requestStream, IServerStreamWriter<SimpleUploadFileResponse> responseStream, ServerCallContext context)
        {
            var internalId = JwtUtil.GetInternalIdFromJwt(context);
            var gameSaveFile = _dbContext.GameSaveFiles.Single(x => x.Id == internalId);
            if (gameSaveFile.Status == GameSaveFileStatus.Stored)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Current game save has been stored."));
            var fileMetadata = _dbContext.FileMetadatas.Single(x => x.Id == internalId);
            // two stream, one for Sha256, the other for Minio
            var pipeStreamServer4Sha256 = new AnonymousPipeServerStream();
            var pipeStreamClient4Sha256 = new AnonymousPipeClientStream(pipeStreamServer4Sha256.GetClientHandleAsString());
            var pipeStreamServer4Minio = new AnonymousPipeServerStream();
            var pipeStreamClient4Minio = new AnonymousPipeClientStream(pipeStreamServer4Minio.GetClientHandleAsString());
            //using var memoryStream = new MemoryStream();
            //long receivedBytes = 0;
            _logger.LogDebug($"SimpleUploadFile: Starting readTask");
            var readTask = Task.Run(async () =>
            {
                gameSaveFile.Status = GameSaveFileStatus.InProgress;
                await _dbContext.SaveChangesAsync();
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    _logger.LogDebug($"SimpleUploadFile: readTask message.Data.Length = {message.Data?.Length}");
                    if (message.Data != null)
                    {
                        //await memoryStream.WriteAsync(message.Data.Memory);
                        await pipeStreamServer4Sha256.WriteAsync(message.Data.Memory);
                        await pipeStreamServer4Minio.WriteAsync(message.Data.Memory);
                        //receivedBytes += message.Data.Length;
                    }
                }
                // must close pipeStream
                pipeStreamServer4Sha256.Close();
                pipeStreamServer4Minio.Close();
            });
            var writeTaskCancellationTokenSource = new CancellationTokenSource();
            _logger.LogDebug($"SimpleUploadFile: Starting writeTask");
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
            var minioClient = MinioClientUtil.GetMinioClient();
            var putObjectArgs = new PutObjectArgs()
                                    .WithBucket(GlobalContext.SystemConfig.MinioBucket)
                                    .WithObject(internalId.ToString())
                                    .WithStreamData(pipeStreamClient4Minio)
                                    // https://youtu.be/V_T8x1n358U?t=348, set size = -1, take all
                                    .WithObjectSize(-1);
            _logger.LogDebug($"SimpleUploadFile: Starting minioPutTask");
            var minioPutTask = minioClient.PutObjectAsync(putObjectArgs);
            // calc Sha256
            using var sha256 = SHA256.Create();
            _logger.LogDebug($"SimpleUploadFile: Starting sha256Task");
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
                gameSaveFile.Status = GameSaveFileStatus.Sha256Mismatch;
                // throw rpcex
                throw new RpcException(new Status(StatusCode.InvalidArgument, "SHA256 digest not match."));
            }
            // cancel writeTask(cdn keep alive)
            writeTaskCancellationTokenSource.Cancel();
            // update db
            var ret = new SimpleUploadFileResponse();
            if (gameSaveFile.Status == GameSaveFileStatus.InProgress)
            {
                gameSaveFile.Status = GameSaveFileStatus.Stored;
                ret.Status = FileTransferStatus.Success;
            }
            else
            {
                ret.Status = FileTransferStatus.Failed;
            }
            gameSaveFile.UpdatedAt = DateTime.Now;
            _dbContext.SaveChanges();
            await responseStream.WriteAsync(ret);
        }
    }
}
