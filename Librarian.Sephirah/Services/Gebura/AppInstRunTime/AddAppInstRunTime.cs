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
        public override Task<AddAppInstRunTimeResponse> AddAppInstRunTime(AddAppInstRunTimeRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var timeRangeReq = request.TimeRange;
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
            appInst.AppInstRunTimes.Add(new AppInstRunTime
            {
                StartDateTime = timeRangeReq.StartTime.ToDateTime(),
                EndDateTime = timeRangeReq.StartTime.ToDateTime() + timeRangeReq.Duration.ToTimeSpan(),
                Duration = timeRangeReq.Duration.ToTimeSpan()
            });
            // add to AppInst TotalRunTime
            appInst.TotalRunTime += timeRangeReq.Duration.ToTimeSpan();
            // add to App TotalRunTime
            appInst.App.TotalRunTime += timeRangeReq.Duration.ToTimeSpan();
            _dbContext.SaveChanges();
            return Task.FromResult(new AddAppInstRunTimeResponse());
        }
    }
}
