using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Handlers;
using Microsoft.Extensions.Logging;
using SteamWebAPI2.Utilities;
using System.Text.Json;
using System.Threading.RateLimiting;

namespace Librarian.ThirdParty.Steam
{
    public partial class SteamApiService : IAppInfoService
    {
        private static readonly JsonSerializerOptions s_jso_urje = new() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        private readonly string _steamAPIKey;
        private readonly SteamWebInterfaceFactory _webInterfaceFactory;
        private readonly HttpClient _httpClient;
        private readonly ILogger<SteamApiService> _logger;

        public string CurrencyCode { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;

        public SteamApiService(string steamAPIKey, TimeSpan minRequestInterval, ILogger<SteamApiService> logger)
        {
            _steamAPIKey = steamAPIKey;
            _logger = logger;
            _webInterfaceFactory = new SteamWebInterfaceFactory(_steamAPIKey);
            _httpClient = new HttpClient(new ClientSideRateLimitedHandler(
                new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 1,
                    TokensPerPeriod = 1,
                    ReplenishmentPeriod = minRequestInterval,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 1,
                    AutoReplenishment = true
                })));
        }
    }
}
