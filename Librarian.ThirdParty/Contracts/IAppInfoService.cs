using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.ThirdParty.Contracts
{
    public interface IAppInfoService
    {
        Task<AppInfo> GetAppInfoAsync(string appId, CancellationToken cts = default);
    }
}
