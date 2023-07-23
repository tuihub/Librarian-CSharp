using Librarian.Common.Models;

namespace Librarian.Common.Utils
{
    public static class GlobalContext
    {
        public static SystemConfig SystemConfig { get; set; } = null!;
        public static JwtConfig JwtConfig { get; set; } = null!;
    }
}
