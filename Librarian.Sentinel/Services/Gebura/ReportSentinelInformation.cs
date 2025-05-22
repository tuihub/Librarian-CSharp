using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1.Sentinel;

namespace Librarian.Sentinel.Services
{
    public partial class SentinelService
    {
        public async override Task<ReportSentinelInformationResponse> ReportSentinelInformation(ReportSentinelInformationRequest request, ServerCallContext context)
        {
            var sentinelId = context.GetInternalIdFromHeader();
            var sentinel = await _dbContext.Sentinels
                .Include(s => s.SentinelLibraries)
                .SingleOrDefaultAsync(e => e.Id == sentinelId, context.CancellationToken);

            if (sentinel == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Sentinel with ID {sentinelId} not found"));
            }
            else
            {
                _logger.LogInformation("Updating existing Sentinel with ID: {SentinelId}", sentinelId);

                sentinel.Update(request);
            }

            await _dbContext.SaveChangesAsync(context.CancellationToken);
            return new ReportSentinelInformationResponse();
        }
    }
}
