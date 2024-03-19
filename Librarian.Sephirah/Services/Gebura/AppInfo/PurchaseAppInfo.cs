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
        public override Task<PurchaseAppInfoResponse> PurchaseAppInfo(PurchaseAppInfoRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            Common.Models.Db.AppInfo? appInfo;
            if (request.AppInfoId.Internal)
            {
                appInfo = _dbContext.AppInfos.SingleOrDefault(x =>
                    x.Id == long.Parse(request.AppInfoId.SourceAppId));
            }
            else
            {
                appInfo = _dbContext.AppInfos.SingleOrDefault(x =>
                    x.Source == request.AppInfoId.Source
                    && x.SourceAppId == request.AppInfoId.SourceAppId);
            }
            if (appInfo == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfo not exists."));
            }
            if (!appInfo.IsInternal)
            {
                var appInfoParent = appInfo.ParentAppInfo;
                if (appInfoParent == null)
                {
                    appInfoParent = new Common.Models.AppInfo
                    {
                        Id = _idGenerator.CreateId(),
                        Source = Common.Constants.Proto.AppInfoSourceInternal,
                        Name = appInfo.Name,
                        Type = appInfo.Type
                    };
                    _dbContext.AppInfos.Add(appInfoParent);
                    appInfo = appInfoParent;
                }
            }
            var user = _dbContext.Users.Single(x => x.Id == userId);
            user.AppInfos.Add(appInfo);
            _dbContext.SaveChanges();
            return Task.FromResult(new PurchaseAppInfoResponse
            {
                Id = new InternalID { Id = appInfo.Id }
            });
        }
    }
}
