using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override async Task<SyncExternalAppsResponse> SyncExternalApps(SyncExternalAppsRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            if (UserUtil.GetUserTypeFromJwt(context, _dbContext) != UserType.Admin)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            // get request param
            foreach (var externalApp in request.ExternalApps)
            {
                if (externalApp == null) continue;
                var appSource = externalApp.Source;
                var appId = externalApp.SourceAppId;
                if (appSource == TuiHub.Protos.Librarian.V1.AppSource.Unspecified ||
                    appSource == TuiHub.Protos.Librarian.V1.AppSource.Internal)
                    continue;
                if (string.IsNullOrWhiteSpace(appId)) continue;

                var app = _dbContext.Apps.SingleOrDefault(x => x.Source == appSource && x.SourceAppId == appId);
                // external app not exists
                if (app == null)
                {
                    var newInternalApp = new Common.Models.App
                    {
                        Id = IdUtil.NewId(),
                        Source = TuiHub.Protos.Librarian.V1.AppSource.Internal,
                        Name = externalApp.Source.ToString() + " " + externalApp.SourceAppId,
                        Type = TuiHub.Protos.Librarian.V1.AppType.Game
                    };
                    var newExternalApp = new Common.Models.App
                    {
                        Id = IdUtil.NewId(),
                        Source = appSource,
                        SourceAppId = appId,
                        Name = string.Empty,
                        Type = TuiHub.Protos.Librarian.V1.AppType.Game,
                        ParentApp = newInternalApp
                    };
                    _dbContext.Apps.Add(newInternalApp);
                    _dbContext.Apps.Add(newExternalApp);
                    await _dbContext.SaveChangesAsync();
                    _pullMetadataService.AddPullApp(newExternalApp.Id);
                }
                // external app exists
                else
                {
                    _pullMetadataService.AddPullApp(app.Id);
                }
            }
            return new SyncExternalAppsResponse();
        }
    }
}
