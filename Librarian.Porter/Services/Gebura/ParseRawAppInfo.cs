using Grpc.Core;
using Librarian.ThirdParty.Contracts;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService
    {
        public override async Task<ParseRawAppInfoResponse> ParseRawAppInfo(ParseRawAppInfoRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Parsing raw app info for source: {Source}, appId: {AppId}", request.Source, request.SourceAppId);

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

            try
            {
                var appInfo = await appInfoService.ParseRawAppInfoAsync(request.SourceAppId, request.RawDataJson, context.CancellationToken);

                _logger.LogInformation("Successfully parsed app info: {AppName}", appInfo.Name);

                return new ParseRawAppInfoResponse
                {
                    AppInfo = appInfo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error parsing raw app info: {Error}", ex.Message);
                throw new RpcException(new Status(StatusCode.Internal,
                    $"Failed to parse raw app info: {ex.Message}"));
            }
        }
    }
}