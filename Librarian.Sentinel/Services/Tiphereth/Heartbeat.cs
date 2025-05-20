using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1.Sentinel;

namespace Librarian.Sentinel.Services
{
    public partial class SentinelService
    {
        // TODO: impl client alive check
        public override Task<HeartbeatResponse> Heartbeat(HeartbeatRequest request, ServerCallContext context)
        {
            var sentinelId = context.GetInternalIdFromHeader();

            _logger.LogInformation("Received heartbeat from Sentinel ID: {SentinelId}, Instance ID: {InstanceId}",
                sentinelId, request.InstanceId);

            return Task.FromResult(new HeartbeatResponse());
        }
    }
}