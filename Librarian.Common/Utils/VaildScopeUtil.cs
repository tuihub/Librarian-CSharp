using Librarian.Common.Models;

namespace Librarian.Common.Utils
{
    public static class VaildScopeUtil
    {
        public static ValidScope FromProtoVaildScope(TuiHub.Protos.Librarian.Sephirah.V1.ValidScope vaildScope)
        {
            return vaildScope switch
            {
                TuiHub.Protos.Librarian.Sephirah.V1.ValidScope.Account => ValidScope.Account,
                TuiHub.Protos.Librarian.Sephirah.V1.ValidScope.App => ValidScope.App,
                TuiHub.Protos.Librarian.Sephirah.V1.ValidScope.AppPackage => ValidScope.AppPackage,
                _ => throw new ArgumentException("Invaild ValidScope")
            };
        }
    }
}
