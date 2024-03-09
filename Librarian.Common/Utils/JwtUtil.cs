using Grpc.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Librarian.Common.Utils
{
    public static class JwtUtil
    {
        public static string GenerateTokenForDownloadServer(long internalId)
        {
            var expireMinutes = 1440;
            var audience = "DLServer";
            var issuer = GlobalContext.JwtConfig.Issuer;

            var handler = new JwtSecurityTokenHandler();
            var ecdsa = ECDsa.Create();
            ecdsa.ImportECPrivateKey(Convert.FromBase64String("MHQCAQEEIChovn0zZY2oz4uZltebqvmEUdKNrutbBvjNsdJfMNP/oAcGBSuBBAAKoUQDQgAEiDwoPSh9/8MOHe2HPjNE6Nz9cTtavMRHQcCrteWpwaiLnXy3BGlDMDhBFdAgr1COEWUh2fvjW40ztPsAguoblA=="), out _);
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Iss, issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, audience),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("internal_id", internalId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(
                    new ECDsaSecurityKey(ecdsa),
                    SecurityAlgorithms.EcdsaSha256Signature)
            };
            var token = handler.CreateToken(descriptor);
            var jwtToken = handler.WriteToken(token);
            return jwtToken;
        }
        public static string GenerateAccessToken(long internalId)
        {
            var expireMinutes = GlobalContext.JwtConfig.AccessTokenExpireMinutes;
            var audience = GlobalContext.JwtConfig.AccessTokenAudience;
            var issuer = GlobalContext.JwtConfig.Issuer;
            return GenerateToken(issuer, audience, expireMinutes, internalId);

        }
        public static string GenerateRefreshToken(long internalId)
        {
            var expireMinutes = GlobalContext.JwtConfig.RefreshTokenExpireMinutes;
            var audience = GlobalContext.JwtConfig.RefreshTokenAudience;
            var issuer = GlobalContext.JwtConfig.Issuer;
            return GenerateToken(issuer, audience, expireMinutes, internalId);
        }
        public static string GenerateUploadToken(long internalId)
        {
            var expireMinutes = GlobalContext.JwtConfig.UploadTokenExpireMinutes;
            var audience = GlobalContext.JwtConfig.UploadTokenAudience;
            var issuer = GlobalContext.JwtConfig.Issuer;
            return GenerateToken(issuer, audience, expireMinutes, internalId);
        }
        public static string GenerateDownloadToken(long internalId)
        {
            var expireMinutes = GlobalContext.JwtConfig.DownloadTokenExpireMinutes;
            var audience = GlobalContext.JwtConfig.DownloadTokenAudience;
            var issuer = GlobalContext.JwtConfig.Issuer;
            return GenerateToken(issuer, audience, expireMinutes, internalId);
        }
        static string GenerateToken(string issuer, string audience, double expireMinutes, long internalId)
        {
            var securityKey = Encoding.UTF8.GetBytes(GlobalContext.JwtConfig.Key);
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Iss, issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, audience),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("internal_id", internalId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(securityKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = handler.CreateToken(descriptor);
            var jwtToken = handler.WriteToken(token);
            return jwtToken;
        }
        public static bool ValidateToken(string token, string audience)
        {
            var key = Encoding.UTF8.GetBytes(GlobalContext.JwtConfig.Key);
            var handler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidIssuer = GlobalContext.JwtConfig.Issuer,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                RequireExpirationTime = true
            };
            var principal = handler.ValidateToken(token, parameters, out _);
            return principal != null;
        }
        public static string? GetBearerToken(this ServerCallContext context)
        {
            var token = context.RequestHeaders.SingleOrDefault(x => x.Key == "authorization")?.Value;
            token = token?.Substring("Bearer ".Length).Trim();
            return token;
        }
        public static long GetInternalIdFromHeader(this ServerCallContext context)
        {
            var token = context.GetBearerToken();
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            var internalId = GetInternalIdFromJwtToken(token);
            return internalId;
        }
        public static long GetInternalIdFromJwtToken(string token)
        {
            return long.Parse(GetClaimFromJwtToken(token, "internal_id")!);
        }
        public static Guid GetJtiFromJwtToken(this string token)
        {
            return Guid.Parse(GetClaimFromJwtToken(token, JwtRegisteredClaimNames.Jti)!);
        }
        private static string? GetClaimFromJwtToken(string token, string claim)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claimValue = jwtToken.Claims.Single(x => x.Type == claim).Value;
            return claimValue;
        }
        public static DateTime GetTokenExpireTime(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        public static void GetJwtBearerOptions(this JwtBearerOptions options, string validAudience)
        {
            var key = Encoding.UTF8.GetBytes(GlobalContext.JwtConfig.Key);
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = GlobalContext.JwtConfig.Issuer,
                ValidAudience = validAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                RequireExpirationTime = true
            };
        }
    }
}
