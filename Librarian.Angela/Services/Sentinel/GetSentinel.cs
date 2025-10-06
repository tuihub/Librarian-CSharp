using Grpc.Core;
using Librarian.Sephirah.Angela;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<GetSentinelResponse> GetSentinel(GetSentinelRequest request,
        ServerCallContext context)
    {
        var sentinel = await _dbContext.Sentinels
            .FirstOrDefaultAsync(x => x.Id == request.Id.Id);

        if (sentinel == null)
            throw new RpcException(new Status(StatusCode.NotFound, "Sentinel not found"));

        // Use AutoMapper to convert entity to protobuf object
        var sentinelPb = _mapper.Map<Sentinel>(sentinel);

        return new GetSentinelResponse
        {
            Sentinel = sentinelPb
        };
    }
}