using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Contracts
{
    public interface IAppInfoService
    {
        Task<AppInfo> GetAppInfoAsync(string appId, CancellationToken ct = default);
        Task<List<AppInfo>> SearchAppInfoAsync(string nameLike, CancellationToken ct = default);
        Task<AppInfo> ParseRawAppInfoAsync(string appId, string rawDataJson, CancellationToken ct = default);
    }
}
