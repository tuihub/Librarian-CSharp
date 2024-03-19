using Librarian.Porter.Configs;
using Librarian.Porter.Constants;
using Librarian.ThirdParty.Bangumi;
using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Steam;
using Librarian.ThirdParty.Vndb;

namespace Librarian.Porter.Server.Utils
{
    public static class ServicesUtil
    {
        public static void ConfigureThirdPartyServices(WebApplicationBuilder builder, PorterConfig porterConfig)
        {
            if (porterConfig.IsSteamEnabled)
            {
                builder.Services.AddSingleton(new SteamAPIService(porterConfig.SteamApiKey));
                StaticContext.PorterTags.Add(WellKnownAppInfoSource.Steam);
            }
            if (porterConfig.IsBangumiEnabled)
            {
                builder.Services.AddSingleton(new BangumiAPIService(porterConfig.BangumiApiKey));
                StaticContext.PorterTags.Add(WellKnownAppInfoSource.Bangumi);
            }
            if (porterConfig.IsVndbEnabled)
            {
                builder.Services.AddSingleton(new VndbTcpAPIService());
                StaticContext.PorterTags.Add(WellKnownAppInfoSource.Vndb);
            }
        }
    }
}
