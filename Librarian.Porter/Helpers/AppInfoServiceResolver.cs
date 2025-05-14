using Librarian.Common.Constants;
using Librarian.Common.Converters;
using Librarian.Porter.Configs;
using Librarian.ThirdParty.Bangumi;
using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Steam;
using Librarian.ThirdParty.Vndb;

namespace Librarian.Porter.Helpers
{
    public class AppInfoServiceResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PorterConfig _porterConfig;
        private readonly SteamAPIService _steamAPIService;
        private readonly BangumiAPIService _bangumiAPIService;
        private readonly VndbTcpAPIService _vndbAPIService;

        public AppInfoServiceResolver(
            IServiceProvider serviceProvider,
            PorterConfig porterConfig,
            SteamAPIService steamAPIService,
            BangumiAPIService bangumiAPIService,
            VndbTcpAPIService vndbAPIService)
        {
            _serviceProvider = serviceProvider;
            _porterConfig = porterConfig;
            _steamAPIService = steamAPIService;
            _bangumiAPIService = bangumiAPIService;
            _vndbAPIService = vndbAPIService;
        }

        public IAppInfoService GetService(string source)
        {
            WellKnowns.AppInfoSource appInfoSource;
            try
            {
                appInfoSource = source.ToEnum<WellKnowns.AppInfoSource>();
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Invalid app info source: {source}");
            }
            if (!IsSourceEnabled(appInfoSource))
            {
                throw new InvalidOperationException("The source is not enabled.");
            }
            return GetAppInfoService(appInfoSource);
        }

        private IAppInfoService GetAppInfoService(WellKnowns.AppInfoSource source)
        {
            return source switch
            {
                WellKnowns.AppInfoSource.Steam => _steamAPIService,
                WellKnowns.AppInfoSource.Bangumi => _bangumiAPIService,
                WellKnowns.AppInfoSource.Vndb => _vndbAPIService,
                _ => throw new ArgumentException($"Unsupported source: {source}")
            };
        }

        private bool IsSourceEnabled(WellKnowns.AppInfoSource source)
        {
            return source switch
            {
                WellKnowns.AppInfoSource.Steam => _porterConfig.IsSteamEnabled,
                WellKnowns.AppInfoSource.Bangumi => _porterConfig.IsBangumiEnabled,
                WellKnowns.AppInfoSource.Vndb => _porterConfig.IsVndbEnabled,
                _ => false
            };
        }
    }
}
