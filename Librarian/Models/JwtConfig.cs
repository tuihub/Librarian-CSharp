namespace Librarian.Models
{
    public class JwtConfig
    {
        public string Issuer { get; set; }
        public string AccessTokenAudience { get; set; }
        public string RefreshTokenAudience { get; set; }
        public string Key { get; set; }
        public double AccessTokenExpireMinutes { get; set; }
        public double RefreshTokenExpireMinutes { get; set; }
    }
}
