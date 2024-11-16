using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Handlers;
using Librarian.ThirdParty.Helpers;
using Steam.Models.DOTA2;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.ThirdParty.Steam
{
    public partial class SteamAPIService: IAppInfoService
    {
        private readonly string _steamAPIKey;
        private readonly SteamWebInterfaceFactory _webInterfaceFactory;
        private readonly HttpClient _httpClient;

        public string CurrencyCode { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;

        public SteamAPIService(string steamAPIKey, TimeSpan minRequestInterval)
        {
            _steamAPIKey = steamAPIKey;
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
