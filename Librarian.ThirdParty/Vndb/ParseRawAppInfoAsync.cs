using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Vndb
{
    public partial class VndbTcpApiService
    {
        public Task<AppInfo> ParseRawAppInfoAsync(string appId, string rawDataJson, CancellationToken ct = default)
        {
            throw new PlatformNotSupportedException("Vndb does not support ParseRawAppInfoAsync. Use GetAppInfoAsync instead.");
        }
    }
}