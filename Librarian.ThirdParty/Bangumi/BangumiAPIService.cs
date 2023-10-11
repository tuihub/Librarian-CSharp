using Librarian.Common.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var response = await client.ExecuteGetAsync(request);
            if (response == null)
                throw new Exception("Bangumi API returned null response.");
            if (response.IsSuccessStatusCode == false)
                throw new Exception("Bangumi API returned non-success status code: " + response.StatusCode.ToString());
            if (response.Content == null)
                throw new Exception("Bangumi API returned null content.");
            dynamic respObj = JObject.Parse(response.Content);
            if (respObj is null)
                throw new Exception("Bangumi API response deserialization returned null object.");
            if (DateTime.TryParse(respObj.date.ToString(), out DateTime releaseDate) == false)
                releaseDate = DateTime.MinValue;
            return new App
            {
                Source = TuiHub.Protos.Librarian.V1.AppSource.Bangumi,
                SourceAppId = respObj.id.ToString(),
                SourceUrl = "https://bgm.tv/subject/" + respObj.id.ToString(),
                Name = string.IsNullOrWhiteSpace(respObj.name_cn.ToString()) ? respObj.name.ToString() : respObj.name_cn.ToString(),
                Type = respObj.type.ToString().Equals("4") ? TuiHub.Protos.Librarian.V1.AppType.Game : TuiHub.Protos.Librarian.V1.AppType.Unspecified,
                ShortDescription = ((string?)respObj.summary?.ToString())?[..97] + "...",
                IconImageUrl = respObj.images?.small?.ToString(),
                HeroImageUrl = respObj.images?.large?.ToString(),
                AppDetails = new AppDetails
                {
                    Description = respObj.summary?.ToString(),
                    ReleaseDate = releaseDate,
                    Developer = respObj.infobox?.GetType().GetProperty("开发")?.GetValue(respObj.infobox, null)?.ToString(),
                    Publisher = null,
                    Version = null
                }
            };
        }
    }
}
