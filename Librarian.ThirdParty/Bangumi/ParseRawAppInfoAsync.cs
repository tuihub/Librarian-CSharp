using Librarian.ThirdParty.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Bangumi
{
    public partial class BangumiApiService
    {
        // constants for Bangumi infobox keys
        private const string DEVELOPER_KEY = "开发";
        private const string ALIAS_KEY = "别名";

        public Task<AppInfo> ParseRawAppInfoAsync(string rawDataJson, CancellationToken ct = default)
        {
            string appIdFromRawInfo = "unknown";
            try
            {
                // parse the raw JSON data
                dynamic respObj = JObject.Parse(rawDataJson);
                appIdFromRawInfo = (string?)respObj?.id?.ToString() ?? "unknown";
                _logger.LogDebug("Parsing raw Bangumi app info data for ID: {AppId}", appIdFromRawInfo);

                if (respObj == null)
                {
                    _logger.LogError("Unable to parse JSON data for Bangumi subject.");
                    throw new ArgumentException("Unable to parse JSON data for Bangumi subject");
                }

                // extract release date
                if (DateTime.TryParse(respObj.date.ToString(), out DateTime releaseDate) == false)
                {
                    _logger.LogWarning("Invalid release date format for Bangumi subject ID: {AppId}", appIdFromRawInfo);
                    releaseDate = DateTime.MinValue;
                }

                // create short description
                var shortDescription = ((string?)respObj.summary?.ToString())?.Length > 97 ?
                    ((string?)respObj.summary?.ToString())?[..97] + "..." : respObj.summary?.ToString();

                // extract developer from infobox
                var infobox = respObj.infobox as JArray;
                string developer = string.Empty;

                if (infobox != null)
                {
                    foreach (JObject parsedObject in infobox.Children<JObject>())
                    {
                        foreach (JProperty parsedProperty in parsedObject.Properties())
                        {
                            if (parsedProperty.Value.ToString().Equals(DEVELOPER_KEY))
                            {
                                developer = parsedProperty?.Parent?.Last?.First?.ToString() ?? string.Empty;
                                _logger.LogDebug("Found developer: {Developer} for ID: {AppId}", developer, appIdFromRawInfo);
                            }
                        }
                    }
                }

                // extract tags
                List<string> tags = [];
                if (respObj.tags?.Any())
                {
                    foreach (var tag in respObj.tags)
                    {
                        var name = tag.name?.ToString();
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            tags.Add(name);
                        }
                    }
                    _logger.LogDebug("Extracted {TagCount} tags for ID: {AppId}", tags.Count, appIdFromRawInfo);
                }

                // extract alternative names
                List<string> altNames = [];
                if (!string.IsNullOrWhiteSpace(respObj.name_cn?.ToString()))
                {
                    if (!string.IsNullOrWhiteSpace(respObj.name?.ToString()))
                    {
                        altNames.Add(respObj.name.ToString());
                    }
                }

                // check for aliases in infobox
                foreach (var info in respObj.infobox)
                {
                    if (info.key == ALIAS_KEY)
                    {
                        foreach (var altName in info.value)
                        {
                            var name = altName.v?.ToString();
                            if (!string.IsNullOrWhiteSpace(name) && !altNames.Contains(name))
                            {
                                altNames.Add(name);
                            }
                        }
                    }
                }
                _logger.LogDebug("Extracted {AltNameCount} alternative names for ID: {AppId}", altNames.Count, appIdFromRawInfo);

                // create AppInfo object
                return Task.FromResult(new AppInfo
                {
                    Source = "bangumi",
                    SourceAppId = respObj.id.ToString(),
                    SourceUrl = "https://bgm.tv/subject/" + respObj.id.ToString(),
                    RawDataJson = rawDataJson,
                    Details = new AppInfoDetails
                    {
                        Description = respObj.summary?.ToString(),
                        ReleaseDate = releaseDate.ToUniversalTime().ToISO8601String(),
                        Developer = developer,
                        Publisher = string.Empty,
                        Version = string.Empty,
                        ImageUrls = { }
                    },
                    Name = (string.IsNullOrWhiteSpace(respObj.name_cn.ToString()) ? respObj.name.ToString() : respObj.name_cn.ToString()) ?? string.Empty,
                    Type = AppType.Game,
                    ShortDescription = shortDescription,
                    IconImageUrl = respObj.images?.small?.ToString(),
                    BackgroundImageUrl = respObj.images?.large?.ToString(),
                    CoverImageUrl = respObj.images?.large?.ToString(),
                    Tags = { tags },
                    NameAlternatives = { altNames }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse Bangumi data for ID: {AppId}. Error: {ErrorMessage}", appIdFromRawInfo, ex.Message);
                throw new ArgumentException($"Failed to parse Bangumi data: {ex.Message}", ex);
            }
        }
    }
}