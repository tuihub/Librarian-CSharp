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
            return GenerateToken(expireMinutes, internalId);

        }
        public static string GenerateRefreshToken(long internalId)
        {
            var expireMinutes = GlobalContext.JwtConfig.RefreshTokenExpireMinutes;
            return GenerateToken(expireMinutes, internalId);
        }
        static string GenerateToken(double expireMinutes, long internalId)
        {
            var securityKey = Encoding.UTF8.GetBytes(GlobalContext.JwtConfig.Key);
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("InternalId", internalId.ToString())
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
    }
}
