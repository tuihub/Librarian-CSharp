using Grpc.Core;
using Librarian.Sephirah.Models;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<ListGameSaveFileResponse> ListGameSaveFile(ListGameSaveFileRequest request, ServerCallContext context)
        {
            var appPackageId = request.AppPackageId.Id;
            var fileMetadatas = _dbContext.GameSaveFiles
                                  .Where(x => x.AppPackageId == appPackageId)
                                  .Join(_dbContext.FileMetadatas,
                                        gameSaveFile => gameSaveFile.Id,
                                        fileMetadata => fileMetadata.Id,
                                        (gameSaveFile, fileMetadata) => fileMetadata)
                                  .ApplyPagingRequest(request.Paging);
            var ret = new ListGameSaveFileResponse
            {
                Paging = new PagingResponse { TotalSize = fileMetadatas.Count() }
            };
            ret.Files.Add(fileMetadatas.Select(x => x.ToProtoFileMetadata()));
            return Task.FromResult(ret);
        }
    }
}
