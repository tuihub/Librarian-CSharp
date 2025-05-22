using Librarian.Common.Constants;
using Librarian.Porter.Models;
using Librarian.ThirdParty.Bangumi;
using Librarian.ThirdParty.Steam;
using Librarian.ThirdParty.Vndb;

namespace Librarian.Porter.Server.Utils
{
    public static class ServicesUtil
    {
        public static void ConfigureThirdPartyServices(WebApplicationBuilder builder, GlobalContext globalContext, ILogger? logger = null)
        {
            var porterConfig = globalContext.PorterConfig;
            var instanceContext = globalContext.InstanceContext;
            if (porterConfig.IsSteamEnabled)
            {
                if (string.IsNullOrEmpty(porterConfig.SteamApiKey))
                {
                    logger?.LogError("Steam API key is not provided.");
                }
                else
                {
                    builder.Services.AddSingleton(new SteamAPIService(porterConfig.SteamApiKey,
                        TimeSpan.FromSeconds(porterConfig.SteamMinRequestIntervalSeconds)));
                    instanceContext.SupportedAccountPlatforms.Add(WellKnowns.AccountPlatform.Steam.ToString().ToLower());
                    instanceContext.SupportedAppInfoSources.Add(WellKnowns.AppInfoSource.Steam.ToString().ToLower());
                }
            }
            if (porterConfig.IsBangumiEnabled)
            {
                if (string.IsNullOrEmpty(porterConfig.BangumiApiKey))
                {
                    logger?.LogError("Bangumi API key is not provided.");
                }
                else
                {
                    builder.Services.AddSingleton(new BangumiAPIService(porterConfig.BangumiApiKey));
                    instanceContext.SupportedAppInfoSources.Add(WellKnowns.AppInfoSource.Bangumi.ToString().ToLower());
                }
            }
            if (porterConfig.IsVndbEnabled)
            {
                builder.Services.AddSingleton(new VndbTcpAPIService());
                instanceContext.SupportedAppInfoSources.Add(WellKnowns.AppInfoSource.Vndb.ToString().ToLower());
            }
        }
    }
}
