using Grpc.Core;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<UpdateAppResponse> UpdateApp(UpdateAppRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            if (UserUtil.GetUserTypeFromJwt(context, _dbContext) != UserType.Admin)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            // check App exists
            var appReq = request.App;
            var app = _dbContext.Apps.SingleOrDefault(x => x.Id == appReq.Id.Id);
            if (app == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "User not exists."));
            // ensure AppSource == APP_SOURCE_INTERNAL
            if (appReq.Source != AppSource.Internal)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppSource must be APP_SOURCE_INTERNAL."));
            // update App
            app.Source = appReq.Source;
            if (appReq.SourceAppId != null) app.SourceAppId = appReq.SourceAppId;
            if (appReq.SourceUrl != null) app.SourceUrl = appReq.SourceUrl;
            if (appReq.Name != null) app.Name = appReq.Name;
            app.Type = appReq.Type;
            if (appReq.ShortDescription != null) app.ShortDescription = appReq.ShortDescription;
            if (appReq.IconImageUrl != null) app.IconImageUrl = appReq.IconImageUrl;
            if (appReq.HeroImageUrl != null) app.HeroImageUrl = appReq.HeroImageUrl;
            if (appReq.Details != null) app.AppDetails = new Models.AppDetails(app.Id, appReq.Details);
            app.UpdatedAt = DateTime.Now;
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppResponse());
        }
    }
}
