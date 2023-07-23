using Librarian.Common.Models;

namespace Librarian.Common.Utils
{
    public static class VaildScopeUtil
    {
        public static VaildScope FromProtoVaildScope(TuiHub.Protos.Librarian.Sephirah.V1.VaildScope vaildScope)
        {
            return vaildScope switch
            {
                TuiHub.Protos.Librarian.Sephirah.V1.VaildScope.Account => VaildScope.Account,
                TuiHub.Protos.Librarian.Sephirah.V1.VaildScope.App => VaildScope.App,
                TuiHub.Protos.Librarian.Sephirah.V1.VaildScope.AppPackage => VaildScope.AppPackage,
                _ => throw new ArgumentException("Invaild ValidScope")
            };
        }
    }
}
