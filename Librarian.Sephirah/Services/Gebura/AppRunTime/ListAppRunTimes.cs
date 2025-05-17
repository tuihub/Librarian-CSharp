using Grpc.Core;
using Librarian.Common.Helpers;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<ListAppRunTimesResponse> ListAppRunTimes(ListAppRunTimesRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var pagingRequest = request.Paging;

            // Check AppIdFilter
            var appIds = request.AppIdFilter.Select(x => x.Id);
            if (appIds.Any() && _dbContext.Apps.Any(x => appIds.Contains(x.Id) && x.UserId != userId))
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "AppIdFilter contains app not owned by user."));
            }

            // Check DeviceIdFilter
            var deviceIds = request.DeviceIdFilter.Select(x => x.Id);
            var userDeviceIds = _dbContext.Users.Include(x => x.Devices).Single(x => x.Id == userId).Devices.Select(x => x.Id);
            if (deviceIds.Any() && deviceIds.Except(userDeviceIds).Any())
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "DeviceIdFilter contains device not owned by user."));
            }

            // Get all user's apps
            var userAppIds = _dbContext.Apps.Where(x => x.UserId == userId).Select(x => x.Id);

            // Build query
            var query = _dbContext.AppRunTimes.AsQueryable();
            if (appIds.Any()) { query = query.Where(x => appIds.Contains(x.AppId)); }
            if (deviceIds.Any()) { query = query.Where(x => deviceIds.Contains(x.DeviceId)); }
            if (request.TimeRangeCross != null)
            {
                var startTime = request.TimeRangeCross.StartTime.ToDateTime();
                var endTime = startTime + request.TimeRangeCross.Duration.ToTimeSpan();
                query = query.Where(x => x.StartDateTime >= startTime && x.StartDateTime <= endTime);
            }
            query = query.OrderByDescending(x => x.StartDateTime);
            query = query.ApplyPagingRequest(request.Paging);
            var appRunTimes = query.ToList();

            // Build response
            var response = new ListAppRunTimesResponse
            {
                Paging = new PagingResponse
                {
                    TotalSize = appRunTimes.Count,
                }
            };
            response.AppRunTimes.AddRange(appRunTimes.Select(x => x.ToPB()));
            return Task.FromResult(response);
        }
    }
}