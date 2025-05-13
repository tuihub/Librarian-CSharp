using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.ThirdParty.Contracts
{
    public interface IAppInfoService
    {
        Task<AppInfo> GetAppInfoAsync(string appId, CancellationToken ct = default);
    }
}
