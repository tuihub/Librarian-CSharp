using System.Diagnostics;
using Google.Protobuf;
using Grpc.Core;
using Librarian.Common;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Minio.DataModel.Args;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    // UNDONE: Current only support AppSaveFile download
    [Authorize(AuthenticationSchemes = "DownloadToken")]
    public override async Task SimpleDownloadFile(SimpleDownloadFileRequest request,
        IServerStreamWriter<SimpleDownloadFileResponse> responseStream, ServerCallContext context)
    {
        var internalId = context.GetInternalIdFromHeader();
        var appSaveFile = _dbContext.AppSaveFiles.Single(x => x.FileMetadataId == internalId);
        if (appSaveFile.Status != AppSaveFileStatus.Stored)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Requested AppSaveFile is not stored."));
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
                        if (bytesRead == 0) break;
                        await responseStream.WriteAsync(new SimpleDownloadFileResponse
                        {
                            Data = UnsafeByteOperations.UnsafeWrap(buffer.AsMemory(0, bytesRead))
                        });
                        Debug.WriteLine($"SimpleDownloadFile: sent {bytesRead} bytes");
                    }
                }).Wait();
            });
        await minioClient.GetObjectAsync(getObjectArgs);
        Debug.WriteLine("SimpleDownloadFile: GetObject finished");
    }
}