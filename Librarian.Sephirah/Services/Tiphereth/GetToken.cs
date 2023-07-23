using Grpc.Core;
using Librarian.Common.Utils;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<GetTokenResponse> GetToken(GetTokenRequest request, ServerCallContext context)
        {
            string accessToken, refreshToken;
            var username = request.Username;
            var password = request.Password;
            // get user
            var user = _dbContext.Users.SingleOrDefault(u => u.Name == username);
            if (user == null)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "User not exists."));
            if (user.Status != UserStatus.Active)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active."));
            if (PasswordHasher.VerifyHashedPassword(user.Password, password))
            {
                // get token
                accessToken = JwtUtil.GenerateAccessToken(user.Id);
                refreshToken = JwtUtil.GenerateRefreshToken(user.Id);
            }
            else
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Username and password not match."));
            }
            return Task.FromResult(new GetTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
