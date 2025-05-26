using Grpc.Core;
using Librarian.ThirdParty.Contracts;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService
    {
        public override async Task<GetAppInfoResponse> GetAppInfo(GetAppInfoRequest request, ServerCallContext context)
        {
            IAppInfoService appInfoService;
            try
            {
                appInfoService = _appInfoServiceResolver.GetService(request.Source);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Invalid app info source: {ex.Message}"));
            }
            var appInfo = await appInfoService.GetAppInfoAsync(request.SourceAppId);
            return new GetAppInfoResponse
            {
                AppInfo = appInfo
            };
        }
    }
}
