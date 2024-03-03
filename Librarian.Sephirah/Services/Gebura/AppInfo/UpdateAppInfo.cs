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
        public override Task<UpdateAppInfoResponse> UpdateAppInfo(UpdateAppInfoRequest request, ServerCallContext context)
        {
            // ensure AppInfoSource == internal
            var appReq = request.AppInfo;
            if (!appReq.Internal)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfoSource must be internal."));
            }
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            // check AppInfo exists
            var appInfo = _dbContext.AppInfos.Include(x => x.AppInfoDetails)
                .SingleOrDefault(x => x.Id == appReq.Id.Id);
            if (appInfo == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfo not exists."));
            }
            // update AppInfo
            appInfo.Source = appReq.Source;
            if (appReq.SourceAppId != null) { appInfo.SourceAppId = appReq.SourceAppId; }
            if (appReq.SourceUrl != null) { appInfo.SourceUrl = appReq.SourceUrl; }
            if (appReq.Name != null) { appInfo.Name = appReq.Name; }
            if (appReq.Type != AppType.Unspecified) { appInfo.Type = appReq.Type; }
            if (appReq.ShortDescription != null) { appInfo.ShortDescription = appReq.ShortDescription; }
            if (appReq.IconImageUrl != null) { appInfo.IconImageUrl = appReq.IconImageUrl; }
            if (appReq.BackgroundImageUrl != null) { appInfo.BackgroundImageUrl = appReq.BackgroundImageUrl; }
            if (appReq.CoverImageUrl != null) { appInfo.CoverImageUrl = appReq.CoverImageUrl; }
            if (appReq.Details != null)
            {
                if (appInfo.AppInfoDetails == null) { appInfo.AppInfoDetails = new Common.Models.AppInfoDetails(appInfo.Id, appReq.Details); }
                else appInfo.AppInfoDetails!.UpdateFromProtoAppInfoDetails(appReq.Details);
            }
            appInfo.UpdatedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppInfoResponse());
        }
    }
}
