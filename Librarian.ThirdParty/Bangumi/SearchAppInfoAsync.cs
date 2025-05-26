using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Bangumi
{
    public partial class BangumiApiService
    {
        public async Task<List<AppInfo>> SearchAppInfoAsync(string nameLike, CancellationToken ct = default)
        {
            _logger.LogInformation("Searching for Bangumi apps with name like: {NameLike}", nameLike);

            // Configure REST client
            var options = new RestClientOptions(_bangumiAPIBaseURL)
            {
                Authenticator = new JwtAuthenticator(_bangumiAPIKey),
                UserAgent = _bangumiAPIUserAgent
            };
            var client = new RestClient(options);

            // Use the search endpoint - type 4 is for games
            var request = new RestRequest("/search/subject/{keywords}")
                .AddUrlSegment("keywords", nameLike)
                .AddQueryParameter("type", 4);

            // Execute request
            var response = await client.ExecuteGetAsync(request, ct);
            ct.ThrowIfCancellationRequested();

            // Check if response is valid
            if (response == null || !response.IsSuccessStatusCode || string.IsNullOrEmpty(response.Content))
            {
                _logger.LogWarning("Bangumi search API returned no valid results: {StatusCode}", response?.StatusCode);
                return new List<AppInfo>();
            }

            // Parse search results
            JArray searchResultsArray = JArray.Parse(response.Content);
            List<AppInfo> results = new();

            _logger.LogInformation("Found {ResultCount} search results from Bangumi", searchResultsArray.Count);

            // Get details for each search result (limited to 10)
            foreach (JToken result in searchResultsArray.Take(10))
            {
                try
                {
                    string appId = result["id"]?.ToString() ?? "";
                    if (string.IsNullOrEmpty(appId))
                    {
                        _logger.LogWarning("Invalid subject ID found in Bangumi search results");
                        continue;
                    }

                    _logger.LogDebug("Retrieving details for Bangumi subject ID: {SubjectId}", appId);

                    // For each search result, get full details
                    var appInfo = await GetAppInfoAsync(appId, ct);
                    results.Add(appInfo);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other results
                    string subjectId = result["id"]?.ToString() ?? "unknown";
                    _logger.LogError(ex, "Error retrieving details for Bangumi subject {SubjectId}: {ErrorMessage}",
                        subjectId, ex.Message);
                }
            }

            _logger.LogInformation("Successfully retrieved {ResultCount} Bangumi app details", results.Count);
            return results;
        }
    }
}