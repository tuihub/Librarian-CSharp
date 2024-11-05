using Grpc.Core;
using Librarian.ThirdParty.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService : LibrarianPorterService.LibrarianPorterServiceBase
    {
        public override async Task<PullAppInfoResponse> PullAppInfo(PullAppInfoRequest request, ServerCallContext context)
        {
            var appInfoId = request.AppInfoId;
            if (appInfoId.Internal)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Internal app info is not supported."));
            }
            IAppInfoService appInfoService;
            try
            {
                appInfoService = _appInfoServiceResolver.GetService(appInfoId.Source);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Invalid app info source: {ex.Message}"));
            }
            var appInfo = await appInfoService.GetAppInfoAsync(appInfoId.SourceAppId);
            return new PullAppInfoResponse
            {
                AppInfo = appInfo
            };
        }
    }
}
