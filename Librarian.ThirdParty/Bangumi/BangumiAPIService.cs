using Librarian.Common.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Librarian.ThirdParty.Bangumi
{
    public class BangumiAPIService
    {
        private readonly string _bangumiAPIKey;

        private readonly string _bangumiAPIBaseURL = "https://api.bgm.tv/";
        private readonly string _bangumiAPIUserAgent = "Librarian-CSharp/0.1 (https://github.com/tuihub/Librarian-CSharp)";

        public BangumiAPIService(string apiKey)
        {
            _bangumiAPIKey = apiKey;
        }

        // TODO: Add cancellation token
        // TODO: Add AltName
        public async Task<App> GetAppAsync(int appId)
        {
            var options = new RestClientOptions(_bangumiAPIBaseURL)
            {
                Authenticator = new JwtAuthenticator(_bangumiAPIKey),
                UserAgent = _bangumiAPIUserAgent
            };
            var client = new RestClient(options);
            var request = new RestRequest("/v0/subjects/{subject_id}")
                                .AddUrlSegment("subject_id", appId);
            var args = new { subject_id = appId };
            var respObj = await client.GetJsonAsync<dynamic>("/v0/subjects/{subject_id}", args);
            if (respObj is null)
                throw new Exception("Bangumi API returned null response.");
            var respJsonObj = respObj as JsonObject;
            if (respJsonObj == null)
                throw new Exception("Bangumi API response is not a JsonObject.");
            if (DateTime.TryParse((string?)respJsonObj["date"], out DateTime releaseDate) == false)
                releaseDate = DateTime.MinValue;
            return new App
            {
                Source = TuiHub.Protos.Librarian.V1.AppSource.Bangumi,
                SourceAppId = respJsonObj["id"]?.ToString(),
                SourceUrl = "https://bgm.tv/subject/" + respJsonObj["id"]?.ToString(),
                Name = (string.IsNullOrWhiteSpace((string?)respJsonObj["name_cn"]) ? respJsonObj["name"]?.ToString()
                            : respJsonObj["name_cn"]?.ToString()) ?? string.Empty,
                Type = (int?)respJsonObj["type"] == 4 ? TuiHub.Protos.Librarian.V1.AppType.Game : TuiHub.Protos.Librarian.V1.AppType.Unspecified,
                ShortDescription = (string?)respJsonObj["summary"] != null ? ((string?)respJsonObj["summary"])![..97] + "..." : null,
                IconImageUrl = (string?)((respJsonObj["images"] as JsonObject)?["small"]),
                HeroImageUrl = (string?)((respJsonObj["images"] as JsonObject)?["large"]),
                AppDetails = new AppDetails
                {
                    Description = (string?)respJsonObj["summary"],
                    ReleaseDate = releaseDate,
                    Developer = (string?)((respJsonObj["infobox"] as JsonObject)?["开发"]),
                    Publisher = null,
                    Version = null
                }
            };
        }
    }
}
