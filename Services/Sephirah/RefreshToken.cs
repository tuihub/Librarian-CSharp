using Grpc.Core;
using Librarian.Utils;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Services.Sephirah
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            string accessTokenNew, refreshTokenNew;
            try
            {
                var refreshToken = context.RequestHeaders.Get("refresh-token").Value;
                var handler = new JwtSecurityTokenHandler();
                if (JwtUtil.ValidateRefreshToken(handler, refreshToken) == true)
                {
                    var token = handler.ReadJwtToken(refreshToken);
                    var internalId = long.Parse(token.Claims.Single(x => x.Type == "internal_id").Value);
                    accessTokenNew = JwtUtil.GenerateAccessToken(internalId);
                    refreshTokenNew = JwtUtil.GenerateRefreshToken(internalId);
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "RefreshToken not vaild."));
                }
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, e.Message));
            }
            return Task.FromResult(new RefreshTokenResponse
            {
                AccessToken = accessTokenNew,
                RefreshToken = refreshTokenNew
            });
        }
    }
}
