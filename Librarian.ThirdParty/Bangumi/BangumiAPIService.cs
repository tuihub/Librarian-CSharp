using Librarian.ThirdParty.Contracts;
using Microsoft.Extensions.Logging;

namespace Librarian.ThirdParty.Bangumi
{
    public partial class BangumiApiService : IAppInfoService
    {
        private readonly string _bangumiApiKey;
        private readonly ILogger<BangumiApiService> _logger;

        private readonly string _bangumiApiBaseURL = "https://api.bgm.tv/";
        private readonly string _bangumiApiUserAgent = "Librarian-CSharp/0.2 (https://github.com/tuihub/Librarian-CSharp)";

        public BangumiApiService(string apiKey, ILogger<BangumiApiService> logger)
        {
            _bangumiApiKey = apiKey;
            _logger = logger;
        }
    }
}
