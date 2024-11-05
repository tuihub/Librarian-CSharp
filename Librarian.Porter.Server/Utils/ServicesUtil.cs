using Librarian.Porter.Configs;
using Librarian.Porter.Constants;
using Librarian.Porter.Models;
using Librarian.ThirdParty.Bangumi;
using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Steam;
using Librarian.ThirdParty.Vndb;
using Microsoft.Extensions.Logging;

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
                    builder.Services.AddSingleton(new SteamAPIService(porterConfig.SteamApiKey));
                    instanceContext.AppInfoSources.Add(WellKnownAppInfoSource.Steam);
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
                    instanceContext.AppInfoSources.Add(WellKnownAppInfoSource.Bangumi);
                }
            }
            if (porterConfig.IsVndbEnabled)
            {
                builder.Services.AddSingleton(new VndbTcpAPIService());
                instanceContext.AppInfoSources.Add(WellKnownAppInfoSource.Vndb);
            }
        }
    }
}
