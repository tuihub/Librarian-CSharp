using Librarian.Sephirah.Models;

namespace Librarian.Sephirah.Utils
{
    public static class GlobalContext
    {
        public static SystemConfig SystemConfig { get; set; }
        public static JwtConfig JwtConfig { get; set; }
    }
}
