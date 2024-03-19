using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Librarian.Common.Models.Db;
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
        public override Task<SumAppInstRunTimeResponse> SumAppInstRunTime(SumAppInstRunTimeRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            if (request.TimeAggregation.AggregationType != TimeAggregation.Types.AggregationType.Overall)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Only AggregationType.Overall is supported."));
            }
            var appInst = _dbContext.AppInsts
                .Where(x => x.Id == request.AppInstId.Id)
                .Include(x => x.App)
                .SingleOrDefault(x => x.Id == request.AppInstId.Id);
            if (appInst == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInst not exists."));
            }
            if (appInst.App.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "AppInst is not owned by user."));
            }
            return Task.FromResult(new SumAppInstRunTimeResponse
            {
                RunTimeGroups = { new SumAppInstRunTimeResponse.Types.Group { Duration = appInst.TotalRunTime.ToDuration() } }
            });
        }
    }
}
