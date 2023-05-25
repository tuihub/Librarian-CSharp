using Grpc.Core;
using Librarian.Sephirah.Models;
using Librarian.Sephirah.Utils;
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
            var appPackageId = request.AppPackageId.Id;
            var appPackage = _dbContext.AppPackages.Single(x => x.Id == appPackageId);
            var startTime = request.TimeRange.StartTime.ToDateTime();
            var duration = request.TimeRange.Duration.ToTimeSpan();
            // update db
            appPackage.TotalRunTime += duration;
            var appPackageRunTime = new AppPackageRunTime
            {
                AppPackage = appPackage,
                StartDateTime = startTime,
                Duration = duration
            };
            _dbContext.AppPackageRunTimes.Add(appPackageRunTime);
            _dbContext.SaveChanges();
            return Task.FromResult(new AddAppPackageRunTimeResponse());
        }
    }
}
