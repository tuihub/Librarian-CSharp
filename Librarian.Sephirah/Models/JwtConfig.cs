namespace Librarian.Sephirah.Models
{
    public class JwtConfig
    {
        public string Issuer { get; set; } = null!;
        public string AccessTokenAudience { get; set; } = null!;
        public string RefreshTokenAudience { get; set; } = null!;
        public string UploadTokenAudience { get; set; } = null!;
        public string DownloadTokenAudience { get; set; } = null!;
        public string Key { get; set; } = null!;
        public double AccessTokenExpireMinutes { get; set; }
        public double RefreshTokenExpireMinutes { get; set; }
        public double BinahTokenExpireMinutes { get; set; }
    }
}
