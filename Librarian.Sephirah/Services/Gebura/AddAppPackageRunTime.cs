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
        public override Task<AddAppPackageRunTimeResponse> AddAppPackageRunTime(AddAppPackageRunTimeRequest request, ServerCallContext context)
        {
            var userId = JwtUtil.GetInternalIdFromJwt(context);
            var appPackageId = request.AppPackageId.Id;
            var appPackage = _dbContext.Apps.Single(x => x.Id == appPackageId);
            var startTime = request.TimeRange.StartTime.ToDateTime();
            var duration = request.TimeRange.Duration.ToTimeSpan();
            // update db
            var userAppPackage = _dbContext.UserAppPackages.Single(x => x.UserId == userId && x.AppInfoId == appPackageId);
            userAppPackage.TotalRunTime += duration;
            var appPackageRunTime = new AppInstRunTime
            {
                UserId = userId,
                AppPackageId = appPackageId,
                StartDateTime = startTime,
                Duration = duration
            };
            _dbContext.UserAppPackageRunTimes.Add(appPackageRunTime);
            _dbContext.SaveChanges();
            return Task.FromResult(new AddAppPackageRunTimeResponse());
        }
    }
}
