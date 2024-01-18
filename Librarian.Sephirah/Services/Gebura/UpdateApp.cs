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
        public override Task<UpdateAppResponse> UpdateApp(UpdateAppRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            // check App exists
            var appReq = request.App;
            var app = _dbContext.Apps.Include(x => x.AppDetails)
                                     .SingleOrDefault(x => x.Id == appReq.Id.Id);
            if (app == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "User not exists."));
            }
            // ensure AppSource == internal
            if (!appReq.Internal)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppSource must be internal."));
            }
            // update App
            app.Source = appReq.Source;
            if (appReq.SourceAppId != null) { app.SourceAppId = appReq.SourceAppId; }
            if (appReq.SourceUrl != null) { app.SourceUrl = appReq.SourceUrl; }
            if (appReq.Name != null) { app.Name = appReq.Name; }
            app.Type = appReq.Type;
            if (appReq.ShortDescription != null) { app.ShortDescription = appReq.ShortDescription; }
            if (appReq.IconImageUrl != null) { app.IconImageUrl = appReq.IconImageUrl; }
            if (appReq.BackgroundImageUrl != null) { app.BackgroundImageUrl = appReq.BackgroundImageUrl; }
            if (appReq.CoverImageUrl != null) { app.CoverImageUrl = appReq.CoverImageUrl; }
            if (appReq.Details != null)
            {
                if (app.AppDetails == null) { app.AppDetails = new Common.Models.AppDetails(app.Id, appReq.Details); }
                else app.AppDetails!.UpdateFromProtoAppDetails(appReq.Details);
            }
            app.UpdatedAt = DateTime.Now;
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppResponse());
        }
    }
}
