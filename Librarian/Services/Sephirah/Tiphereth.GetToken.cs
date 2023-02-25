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
                // get user
                var user = db.Users.SingleOrDefault(u => u.Name == username);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "User not exists."));
                if (user.Status != UserStatus.Active)
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active."));
                if (PasswordHasher.VerifyHashedPassword(user.Password, password))
                {
                    // get token
                    accessToken = JwtUtil.GenerateAccessToken(user.InternalId);
                    refreshToken = JwtUtil.GenerateRefreshToken(user.InternalId);
                }
                else
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Username and password not match."));
                }
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, ex.Message));
            }
            return Task.FromResult(new GetTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
