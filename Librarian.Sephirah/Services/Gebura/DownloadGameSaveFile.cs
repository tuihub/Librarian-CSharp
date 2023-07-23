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
        public override Task<DownloadGameSaveFileResponse> DownloadGameSaveFile(DownloadGameSaveFileRequest request, ServerCallContext context)
        {
            var internalId = request.Id.Id;
            var token = JwtUtil.GenerateDownloadToken(internalId);
            return Task.FromResult(new DownloadGameSaveFileResponse
            {
                DownloadToken = token
            });
        }
    }
}
