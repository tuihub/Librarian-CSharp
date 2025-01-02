using Grpc.Core;
using Librarian.Common.Helpers;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // TODO: impl removal
        [Authorize]
        public override Task<ReportSentinelInformationResponse> ReportSentinelInformation(ReportSentinelInformationRequest request, ServerCallContext context)
        {
            var sentinelId = context.GetInternalIdFromHeader();
            var sentinel = _dbContext.Sentinels.SingleOrDefault(s => s.Id == sentinelId) ?? new Common.Models.Db.Sentinel();
            sentinel.Hostnames = request.Hostnames;
            sentinel.Scheme = request.Scheme.ToConstantServerScheme();
            sentinel.GetTokenUrlPath = request.GetTokenUrlPath;
            sentinel.DownloadFileUrlPath = request.DownloadFileUrlPath;
            if (sentinel.Id == 0)
            {
                sentinel.Id = sentinelId;
                _dbContext.Sentinels.Add(sentinel);
            }
            else
            {
                sentinel.UpdatedAt = DateTime.UtcNow;
            }
            foreach (var library in request.Libraries)
            {
                var sentinelLibrary = _dbContext.SentinelLibraries.SingleOrDefault(x => x.SentinelId == sentinelId && x.LibraryId == library.Id);
                if (sentinelLibrary != null)
                {
                    sentinelLibrary.DownloadBasePath = library.DownloadBasePath;
                    sentinelLibrary.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    sentinel.SentinelLibraries.Add(new Common.Models.Db.SentinelLibrary
                    {
                        LibraryId = library.Id,
                        DownloadBasePath = library.DownloadBasePath,
                        SentinelId = sentinelId
                    });
                }
            }
            _dbContext.SaveChanges();
            return Task.FromResult(new ReportSentinelInformationResponse());
        }
    }
}
