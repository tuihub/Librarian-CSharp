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
            using var db = new ApplicationDbContext();
            var appPackageId = request.AppPackageId.Id;
            var fileMetadatas = db.GameSaveFiles
                                  .Where(x => x.AppPackageId == appPackageId)
                                  .ApplyPagingRequest(request.Paging)
                                  .Join(db.FileMetadatas,
                                        gameSaveFile => gameSaveFile.Id,
                                        fileMetadata => fileMetadata.Id,
                                        (gameSaveFile, fileMetadata) => fileMetadata);
            var ret = new ListGameSaveFileResponse
            {
                Paging = new PagingResponse { TotalSize = fileMetadatas.Count() }
            };
            ret.FileList.Add(fileMetadatas.Select(x => x.ToProtoFileMetadata()));
            return Task.FromResult(ret);
        }
    }
}
