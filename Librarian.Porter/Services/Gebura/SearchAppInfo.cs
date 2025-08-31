using Grpc.Core;
using Librarian.Common.Models.FeatureRequests;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using System.Text.Json;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService
    {
        public override async Task<SearchAppInfoResponse> SearchAppInfo(SearchAppInfoRequest request, ServerCallContext context)
        {
            var validationErrors = JsonSchema.FromType<SearchAppInfo>().Validate(request.Config.ConfigJson);
            if (validationErrors != null && validationErrors.Count > 0)
            {
                var errorMsg = $"SearchAppInfo config validation failed: {string.Join("; ", validationErrors.Select(e => e.ToString()))}";
                _logger.LogWarning(errorMsg);
                throw new RpcException(new Status(StatusCode.InvalidArgument, errorMsg));
            }
            var config = JsonSerializer.Deserialize<SearchAppInfo>(request.Config.ConfigJson)!;

            _logger.LogInformation("Searching app info with name like: {NameLike}", config.NameLike);

            var results = new List<AppInfo>();
            try
            {
                var appInfoService = _appInfoServiceResolver.GetService(request.Config);
                var appInfos = await appInfoService.SearchAppInfoAsync(config.NameLike, context.CancellationToken);
                results.AddRange(appInfos);
            }
            catch (Exception ex)
            {
                // Log the error but continue with other sources
                _logger.LogWarning("Error searching in source {Source}: {Error}", request.Config.Id, ex.Message);
            }

            _logger.LogInformation("Found {Count} app info results", results.Count);

            return new SearchAppInfoResponse
            {
                AppInfos = { results }
            };
        }
    }
}