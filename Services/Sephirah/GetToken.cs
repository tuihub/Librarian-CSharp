using Grpc.Core;
using Librarian.Utils;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Services.Sephirah
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<GetTokenResponse> GetToken(GetTokenRequest request, ServerCallContext context)
        {
            string accessToken, refreshToken;
            try
            {
                using var db = new TestDbContext();
                var username = request.Username;
                var password = request.Password;
                var user = db.Users.Single(u => u.UserName == username);
                if (PasswordHasher.VerifyHashedPassword(user.Password, password))
                {
                    accessToken = JwtUtil.GenerateAccessToken(user.InternalId);
                    refreshToken = JwtUtil.GenerateRefreshToken(user.InternalId);
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Username and password not match."));
                }
            }
            catch (Exception e)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, e.Message));
            }
            return Task.FromResult(new GetTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
