using Librarian.Porter.Configs;
using Librarian.ThirdParty.Bangumi;
using Librarian.ThirdParty.Steam;
using Librarian.ThirdParty.Vndb;

namespace Librarian.Porter.Server.Helpers
{
    public static class ServicesHelper
    {
        public static void ConfigureThirdPartyServices(WebApplicationBuilder builder, PorterConfig porterConfig)
        {
            if (porterConfig.IsSteamEnabled)
            {
                builder.Services.AddSingleton<SteamAPIService>(new SteamAPIService(porterConfig.SteamApiKey));
            }
            if (porterConfig.IsBangumiEnabled)
            {
                builder.Services.AddSingleton<BangumiAPIService>(new BangumiAPIService(porterConfig.BangumiApiKey));
            }
            if (porterConfig.IsVndbEnabled)
            {
                builder.Services.AddSingleton<VndbTcpAPIService>(new VndbTcpAPIService());
            }
        }
    }
}
