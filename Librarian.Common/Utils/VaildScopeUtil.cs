using Librarian.Common.Models;

namespace Librarian.Common.Utils
{
    public static class VaildScopeUtil
    {
        public static EntityType FromProtoVaildScope(TuiHub.Protos.Librarian.Sephirah.V1.ValidScope vaildScope)
        {
            return vaildScope switch
            {
                TuiHub.Protos.Librarian.Sephirah.V1.ValidScope.Account => EntityType.Account,
                TuiHub.Protos.Librarian.Sephirah.V1.ValidScope.App => EntityType.AppInfo,
                TuiHub.Protos.Librarian.Sephirah.V1.ValidScope.AppPackage => EntityType.App,
                _ => throw new ArgumentException("Invaild ValidScope")
            };
        }
    }
}
