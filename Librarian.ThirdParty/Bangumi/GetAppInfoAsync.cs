using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Authenticators;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Bangumi
{
    public partial class BangumiApiService
    {
        public async Task<AppInfo> GetAppInfoAsync(string appIdStr, CancellationToken ct = default)
        {
            if (!int.TryParse(appIdStr, out int appId))
            {
                _logger.LogError("Invalid application ID format: {AppIdStr}", appIdStr);
                throw new ArgumentException("appIdStr must be a valid integer.");
            }

            // Configure REST client
            var options = new RestClientOptions(_bangumiApiBaseURL)
            {
                Authenticator = new JwtAuthenticator(_bangumiApiKey),
                UserAgent = _bangumiApiUserAgent
            };
            var client = new RestClient(options);

            // Create request to get subject details
            var request = new RestRequest("/v0/subjects/{subject_id}")
                            .AddUrlSegment("subject_id", appId);

            _logger.LogInformation("Requesting Bangumi subject details for ID: {AppId}", appId);

            // Execute request
            var response = await client.ExecuteGetAsync(request, ct);
            ct.ThrowIfCancellationRequested();

            // Validate response
            if (response == null)
            {
                _logger.LogError("Bangumi API returned null response for ID: {AppId}", appId);
                throw new Exception("Bangumi API returned null response.");
            }
            if (response.IsSuccessStatusCode == false)
            {
                _logger.LogError("Bangumi API returned non-success status code: {StatusCode} for ID: {AppId}", response.StatusCode, appId);
                throw new Exception("Bangumi API returned non-success status code: " + response.StatusCode.ToString());
            }
            if (response.Content == null)
            {
                _logger.LogError("Bangumi API returned null content for ID: {AppId}", appId);
                throw new Exception("Bangumi API returned null content.");
            }

            _logger.LogDebug("Successfully received Bangumi data for ID: {AppId}", appId);

            // Use ParseRawAppInfoAsync to parse response content
            return await ParseRawAppInfoAsync(appIdStr, response.Content, ct);
        }
    }
}