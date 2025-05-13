using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Helpers;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Bangumi
{
    public class BangumiAPIService : IAppInfoService
    {
        private readonly string _bangumiAPIKey;

        private readonly string _bangumiAPIBaseURL = "https://api.bgm.tv/";
        private readonly string _bangumiAPIUserAgent = "Librarian-CSharp/0.2 (https://github.com/tuihub/Librarian-CSharp)";

        public BangumiAPIService(string apiKey)
        {
            _bangumiAPIKey = apiKey;
        }

        public async Task<AppInfo> GetAppInfoAsync(string appIdStr, CancellationToken ct = default)
        {
            if (!int.TryParse(appIdStr, out int appId))
            {
                throw new ArgumentException("appIdStr must be a valid integer.");
            }
            var options = new RestClientOptions(_bangumiAPIBaseURL)
            {
                Authenticator = new JwtAuthenticator(_bangumiAPIKey),
                UserAgent = _bangumiAPIUserAgent
            };
            var client = new RestClient(options);
            var request = new RestRequest("/v0/subjects/{subject_id}")
                                .AddUrlSegment("subject_id", appId);
            var response = await client.ExecuteGetAsync(request, ct);
            ct.ThrowIfCancellationRequested();
            if (response == null) { throw new Exception("Bangumi API returned null response."); }
            if (response.IsSuccessStatusCode == false) { throw new Exception("Bangumi API returned non-success status code: " + response.StatusCode.ToString()); }
            if (response.Content == null) { throw new Exception("Bangumi API returned null content."); }
            dynamic respObj = JObject.Parse(response.Content);
            if (respObj is null) { throw new Exception("Bangumi API response deserialization returned null object."); }
            if (DateTime.TryParse(respObj.date.ToString(), out DateTime releaseDate) == false) { releaseDate = DateTime.MinValue; }
            var shortDescription = ((string?)respObj.summary?.ToString())?.Length > 97 ?
                ((string?)respObj.summary?.ToString())?[..97] + "..." : respObj.summary?.ToString();
            var infobox = respObj.infobox as JArray;
            string developer = string.Empty;
            if (infobox != null)
            {
                foreach (JObject parsedObject in infobox.Children<JObject>())
                {
                    foreach (JProperty parsedProperty in parsedObject.Properties())
                    {
                        if (parsedProperty.Value.ToString().Equals("开发"))
                        {
                            developer = parsedProperty?.Parent?.Last?.First?.ToString() ?? string.Empty;
                        }
                    }
                }
            }
            // tags
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
            }
            // alt_names
            List<string> altNames = [];
            if (!string.IsNullOrWhiteSpace(respObj.name_cn?.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(respObj.name?.ToString()))
                {
                    altNames.Add(respObj.name.ToString());
                }
            }
            foreach (var info in respObj.infobox)
            {
                if (info.key == "别名")
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
            return new AppInfo
            {
                Source = "bangumi",
                SourceAppId = respObj.id.ToString(),
                SourceUrl = "https://bgm.tv/subject/" + respObj.id.ToString(),
                RawDataJson = response.Content,
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
            };
        }
    }
}
