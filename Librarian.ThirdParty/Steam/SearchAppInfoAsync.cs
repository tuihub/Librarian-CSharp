using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Extensions.Logging;
using RestSharp;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Steam;

public partial class SteamApiService
{
    public async Task<List<AppInfo>> SearchAppInfoAsync(string nameLike, CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Searching for Steam apps with name like: {NameLike}", nameLike);

            // Use InternalSteamAPI's SearchApps endpoint
            var searchUrl = $"https://steamcommunity.com/actions/SearchApps/{HttpUtility.UrlEncode(nameLike)}";

            var client = new RestClient();
            var request = new RestRequest(searchUrl);
            var response = await client.ExecuteAsync(request, ct);

            if (!response.IsSuccessful)
            {
                _logger.LogError("Failed to search Steam apps: {StatusCode} {ErrorMessage}", response.StatusCode,
                    response.ErrorMessage);
                return new List<AppInfo>();
            }

            var searchResults = JsonSerializer.Deserialize<List<SteamAppSearchResult>>(response.Content!);
            if (searchResults == null || searchResults.Count == 0)
            {
                _logger.LogInformation("No Steam apps found matching the search term: {NameLike}", nameLike);
                return new List<AppInfo>();
            }

            _logger.LogInformation("Found {ResultCount} Steam apps matching the search term", searchResults.Count);

            List<AppInfo> results = new();
            foreach (var item in searchResults.Take(10))
                try
                {
                    _logger.LogDebug("Retrieving details for Steam app ID: {AppId}", item.AppId);
                    var appInfo = await GetAppInfoAsync(item.AppId, ct);

                    // If GetAppInfoAsync didn't set the icon URL, set it from the search result
                    if (string.IsNullOrEmpty(appInfo.IconImageUrl) && !string.IsNullOrEmpty(item.Icon))
                        appInfo.IconImageUrl = item.Icon;

                    results.Add(appInfo);
                }
                catch (Exception ex)
                {
                    // Log error but continue with other results
                    _logger.LogError(ex, "Error retrieving details for Steam app {AppId}: {ErrorMessage}", item.AppId,
                        ex.Message);
                }

            _logger.LogInformation("Successfully retrieved {ResultCount} Steam app details", results.Count);
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Steam apps: {ErrorMessage}", ex.Message);
            return new List<AppInfo>();
        }
    }

    // Model for InternalSteamAPI SearchApps endpoint response
    private class SteamAppSearchResult
    {
        [JsonPropertyName("appid")] public string AppId { get; } = string.Empty;

        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

        [JsonPropertyName("icon")] public string Icon { get; } = string.Empty;

        [JsonPropertyName("logo")] public string Logo { get; set; } = string.Empty;
    }

    // Keep original classes for compatibility with existing code
    private class SteamStoreSearchResponse
    {
        public StoreSearchItem[] Items { get; set; } = Array.Empty<StoreSearchItem>();
    }

    private class StoreSearchItem
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}