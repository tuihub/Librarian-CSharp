using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Librarian.Utils
{
    public static class JwtUtil
    {
        public static string GenerateAccessToken(long internalId)
        {
            var expireMinutes = GlobalContext.JwtConfig.AccessTokenExpireMinutes;
            var audience = GlobalContext.JwtConfig.AccessTokenAudience;
            return GenerateToken(audience, expireMinutes, internalId);

        }
        public static string GenerateRefreshToken(long internalId)
        {
            var expireMinutes = GlobalContext.JwtConfig.RefreshTokenExpireMinutes;
            var audience = GlobalContext.JwtConfig.RefreshTokenAudience;
            return GenerateToken(audience, expireMinutes, internalId);
        }
        static string GenerateToken(string audience, double expireMinutes, long internalId)
        {
            var securityKey = Encoding.UTF8.GetBytes(GlobalContext.JwtConfig.Key);
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
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
        //public static bool ValidateRefreshToken(JwtSecurityTokenHandler handler, string token)
        //{
        //    var key = Encoding.UTF8.GetBytes(GlobalContext.JwtConfig.Key);
        //    var parameters = new TokenValidationParameters
        //    {
        //        ValidateIssuer = false,
        //        //ValidIssuer = GlobalContext.JwtConfig.Issuer,
        //        ValidAudience = GlobalContext.JwtConfig.RefreshTokenAudience,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(key),
        //        RequireExpirationTime = true
        //    };
        //    var principal = handler.ValidateToken(token, parameters, out _);
        //    return principal != null;
        //}
        public static long GetInternalIdFromToken(string token)
        {
            token = token.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var internalId = long.Parse(jwtToken.Claims.Single(x => x.Type == "internal_id").Value);
            return internalId;
        }
    }
}
