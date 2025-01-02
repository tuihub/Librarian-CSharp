using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Helpers;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using TuiHub.Protos.Librarian.V1;

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

        // TODO: Add AltName
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
            string? developer = null;
            if (infobox != null)
            {
                foreach (JObject parsedObject in infobox.Children<JObject>())
                {
                    foreach (JProperty parsedProperty in parsedObject.Properties())
                    {
                        if (parsedProperty.Value.ToString().Equals("开发"))
                        {
                            developer = parsedProperty?.Parent?.Last?.First?.ToString();
                        }
                    }
                }
            }
            return new AppInfo
            {
                Source = "bangumi",
                SourceAppId = respObj.id.ToString(),
                SourceUrl = "https://bgm.tv/subject/" + respObj.id.ToString(),
                Name = string.IsNullOrWhiteSpace(respObj.name_cn.ToString()) ? respObj.name.ToString() : respObj.name_cn.ToString(),
                Type = respObj.type.ToString().Equals("4") ? AppType.Game : AppType.Unspecified,
                ShortDescription = shortDescription,
                IconImageUrl = respObj.images?.small?.ToString(),
                CoverImageUrl = respObj.images?.large?.ToString(),
                BackgroundImageUrl = respObj.images?.large?.ToString(),
                Details = new AppInfoDetails
                {
                    Description = respObj.summary?.ToString(),
                    ReleaseDate = releaseDate.ToUniversalTime().ToISO8601String(),
                    Developer = developer,
                    Publisher = null,
                    Version = null
                }
            };
        }
    }
}
