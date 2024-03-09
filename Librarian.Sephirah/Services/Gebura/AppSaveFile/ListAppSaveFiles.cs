using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<ListAppSaveFilesResponse> ListAppSaveFiles(ListAppSaveFilesRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var appId = request.AppId.Id;
            var app = _dbContext.Apps.Single(x => x.Id == appId);
            if (app.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "You do not have permission to list save files for this app."));
            }
            var appSaveFiles = _dbContext.AppSaveFiles
                .Where(x => x.AppId == appId)
                .Include(x => x.FileMetadata);
            return Task.FromResult(new ListAppSaveFilesResponse
            {
                Results = { appSaveFiles.Select(x => new ListAppSaveFilesResponse.Types.Result
                {
                    File = x.FileMetadata.ToProtoFileMetadata(),
                    Pinned = x.IsPinned
                }) }
            });
        }
    }
}
