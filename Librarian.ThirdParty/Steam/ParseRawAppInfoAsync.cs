using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Steam
{
    public partial class SteamApiService
    {
        public Task<AppInfo> ParseRawAppInfoAsync(string appId, string rawDataJson, CancellationToken ct = default)
        {
            throw new PlatformNotSupportedException("Steam does not support ParseRawAppInfoAsync. Use GetAppInfoAsync instead.");
        }
    }
}