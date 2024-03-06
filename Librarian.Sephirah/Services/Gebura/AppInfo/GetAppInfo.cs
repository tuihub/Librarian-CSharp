using Grpc.Core;
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
        public override Task<GetAppInfoResponse> GetAppInfo(GetAppInfoRequest request, ServerCallContext context)
        {
            var appInfoId = request.AppInfoId.Id;
            var appInfo = _dbContext.AppInfos
                .Where(x => x.Id == appInfoId)
                .Include(x => x.AppInfoDetails)
                .Include(x => x.ChildAppInfos)
                .ThenInclude(x => x.AppInfoDetails)
                .SingleOrDefault(x => x.Id == appInfoId);
            if (appInfo == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfo not exists."));
            }
            return Task.FromResult(new GetAppInfoResponse
            {
                AppInfo = appInfo.Flatten().ToProtoAppInfo()
            });
        }
    }
}
