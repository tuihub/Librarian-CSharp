using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // tODO: impl removal
        [Authorize]
        public override Task<ReportAppBinariesResponse> ReportAppBinaries(ReportAppBinariesRequest request, ServerCallContext context)
        {
            var sentinelId = context.GetInternalIdFromHeader();
            foreach (var sentinelAppBinary in request.SentinelAppBinaries)
            {
                var sentinelLibrary = _dbContext.SentinelLibraries.SingleOrDefault(x => x.SentinelId == sentinelId && x.LibraryId == sentinelAppBinary.SentinelLibraryId);
                if (sentinelLibrary == null)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Sentinel library not found"));
                }
                var appBinary = _dbContext.AppBinaries.SingleOrDefault(x => x.SentinelGeneratedId == sentinelAppBinary.SentinelGeneratedId);
                if (appBinary == null)
                {
                    _dbContext.AppBinaries.Add(new Common.Models.Db.AppBinary(sentinelAppBinary.AppBinary, _idGenerator.CreateId(),
                        sentinelLibrary.Id, sentinelAppBinary.SentinelGeneratedId));
                }
                else
                {
                    //appBinary.UpdateFromProto(sentinelAppBinary.AppBinary,
                    //    _dbContext.SentinelLibraries.Single(x => x.SentinelId == sentinelId && x.LibraryId == sentinelAppBinary.SentinelLibraryId).Id);
                    _dbContext.AppBinaries.Remove(appBinary);
                    _dbContext.AppBinaries.Add(new Common.Models.Db.AppBinary(sentinelAppBinary.AppBinary, _idGenerator.CreateId(),
                        sentinelLibrary.Id, sentinelAppBinary.SentinelGeneratedId));
                }
            }
            _dbContext.SaveChanges();
            return Task.FromResult(new ReportAppBinariesResponse());
        }
    }
}
