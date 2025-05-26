using Librarian.ThirdParty.Helpers;
using Librarian.ThirdParty.Vndb.Helpers;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TuiHub.Protos.Librarian.Porter.V1;
using VndbSharp;

namespace Librarian.ThirdParty.Vndb
{
    public partial class VndbTcpApiService
    {
        public async Task<List<AppInfo>> SearchAppInfoAsync(string nameLike, CancellationToken ct = default)
        {
            _logger.LogInformation("Searching for visual novels with name like: {NameLike}", nameLike);

            // Create and configure client
            var client = CreateVndbClient();

            // Search for visual novels by fuzzy name match
            var searchResults = await client.GetVisualNovelAsync(VndbFilters.Search.Fuzzy(nameLike), VndbSharp.Models.VndbFlags.FullVisualNovel);

            if (searchResults == null)
            {
                _logger.LogInformation("No visual novels found matching the search term: {NameLike}", nameLike);
                return new List<AppInfo>();
            }

            _logger.LogInformation("Found {ResultCount} visual novels matching the search term", searchResults.Count);

            List<AppInfo> results = new();
            foreach (var vn in searchResults.Take(10))
            {
                try
                {
                    _logger.LogDebug("Processing VNDB result for visual novel ID: {VnId}", vn.Id);

                    // Create short description with max 97 characters
                    var shortDescription = vn.Description.Length > 97 ?
                        vn.Description[..97] + "..." : vn.Description;

                    results.Add(new AppInfo
                    {
                        Source = "vndb",
                        SourceAppId = vn.Id.ToString(),
                        SourceUrl = "https://vndb.org/v" + vn.Id.ToString(),
                        RawDataJson = JsonSerializer.Serialize(vn, s_jso_urje),
                        Details = new AppInfoDetails
                        {
                            Description = vn.Description,
                            ReleaseDate = vn.Released.ToDateTime().ToUniversalTime().ToISO8601String(),
                            Developer = string.Empty,
                            Publisher = string.Empty,
                            Version = string.Empty,
                            ImageUrls = { vn.Screenshots.Select(x => x.Url) }
                        },
                        Name = vn.OriginalName,
                        Type = AppType.Game,
                        ShortDescription = shortDescription,
                        IconImageUrl = string.Empty,
                        BackgroundImageUrl = string.Empty,
                        CoverImageUrl = vn.Image,
                        Tags = { },
                        NameAlternatives = { vn.Aliases }
                    });
                }
                catch (Exception ex)
                {
                    // Log error but continue with other results
                    _logger.LogError(ex, "Error processing VNDB result for visual novel {VnId}: {ErrorMessage}", vn.Id, ex.Message);
                }
            }

            _logger.LogInformation("Successfully processed {ResultCount} visual novel details", results.Count);
            return results;
        }
    }
}