using Grpc.Core;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService
    {
        public override async Task<SearchAppInfoResponse> SearchAppInfo(SearchAppInfoRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Searching app info with name like: {NameLike}", request.NameLike);

            // Get all available services and search across them
            var results = new List<AppInfo>();
            // TODO: !!get from static config or consul
            var sources = new List<string> { "steam", "bangumi", "vndb" }; // Available sources

            foreach (var source in sources)
            {
                try
                {
                    var appInfoService = _appInfoServiceResolver.GetService(source);
                    var appInfos = await appInfoService.SearchAppInfoAsync(request.NameLike, context.CancellationToken);
                    results.AddRange(appInfos);
                }
                catch (Exception ex)
                {
                    // Log the error but continue with other sources
                    _logger.LogWarning("Error searching in source {Source}: {Error}", source, ex.Message);
                }
            }

            _logger.LogInformation("Found {Count} app info results", results.Count);

            return new SearchAppInfoResponse
            {
                AppInfos = { results }
            };
        }
    }
}