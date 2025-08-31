using Grpc.Core;
using Librarian.Common.Models.FeatureRequests;
using Librarian.ThirdParty.Contracts;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using System.Text.Json;
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
                appInfoService = _appInfoServiceResolver.GetService(request.Config);
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument,
                    $"Invalid app info source: {ex.Message}"));
            }
            var validationErrors = JsonSchema.FromType<GetAppInfo>().Validate(request.Config.ConfigJson);
            if (validationErrors != null && validationErrors.Count != 0)
            {
                var errorMsg = $"AppInfo config validation failed: {string.Join("; ", validationErrors.Select(e => e.ToString()))}";
                _logger.LogWarning(errorMsg);
                throw new RpcException(new Status(StatusCode.InvalidArgument, errorMsg));
            }
            var config = JsonSerializer.Deserialize<GetAppInfo>(request.Config.ConfigJson)!;
            var appInfo = await appInfoService.GetAppInfoAsync(config.AppId);
            return new GetAppInfoResponse
            {
                AppInfo = appInfo
            };
        }
    }
}
