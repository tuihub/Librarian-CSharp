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
        public override Task<ListGameSaveFilesResponse> ListGameSaveFiles(ListGameSaveFilesRequest request, ServerCallContext context)
        {
            var userId = JwtUtil.GetInternalIdFromJwt(context);
            var appPackageId = request.AppPackageId.Id;
            var fileMetadatas = _dbContext.GameSaveFiles
                                  .Where(x => x.AppPackageId == appPackageId &&
                                              x.UserId == userId)
                                  .Join(_dbContext.FileMetadatas,
                                        gameSaveFile => gameSaveFile.Id,
                                        fileMetadata => fileMetadata.Id,
                                        (gameSaveFile, fileMetadata) => new
                                        {
                                            FileMetadata = fileMetadata,
                                            IsPinned = gameSaveFile.IsPinned
                                        })
                                  .ApplyPagingRequest(request.Paging);
            var ret = new ListGameSaveFilesResponse
            {
                Paging = new PagingResponse { TotalSize = fileMetadatas.Count() }
            };
            ret.Results.Add(fileMetadatas.Select(x => new ListGameSaveFilesResponse.Types.Result
            {
                File = x.FileMetadata.ToProtoFileMetadata(),
                Pinned = x.IsPinned
            }));
            return Task.FromResult(ret);
        }
    }
}
