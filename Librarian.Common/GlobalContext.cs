using Librarian.Common.Configs;

namespace Librarian.Common
{
    public static class GlobalContext
    {
        public static SystemConfig SystemConfig { get; set; } = null!;
        public static JwtConfig JwtConfig { get; set; } = null!;
        public static InstanceConfig InstanceConfig { get; set; } = null!;
    }
}
