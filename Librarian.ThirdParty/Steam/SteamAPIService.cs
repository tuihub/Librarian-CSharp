using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.RateLimiting;
using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Handlers;
using Microsoft.Extensions.Logging;
using SteamWebAPI2.Utilities;

namespace Librarian.ThirdParty.Steam;

public partial class SteamApiService : IAppInfoService
{
    private static readonly JsonSerializerOptions s_jso_urje = new()
        { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    private readonly HttpClient _httpClient;
    private readonly ILogger<SteamApiService> _logger;

    private readonly string _steamApiKey;
    private readonly SteamWebInterfaceFactory _webInterfaceFactory;

    public SteamApiService(string steamApiKey, TimeSpan minRequestInterval, ILogger<SteamApiService> logger)
    {
        _steamApiKey = steamApiKey;
        _logger = logger;
        _webInterfaceFactory = new SteamWebInterfaceFactory(_steamApiKey);
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

    public string CurrencyCode { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}