using Grpc.Core;
using Librarian.Common.Utils;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // TODO: reduce token expiration time when DeviceId is not present
        public override Task<GetTokenResponse> GetToken(GetTokenRequest request, ServerCallContext context)
        {
            string accessToken, refreshToken;
            var username = request.Username;
            var password = request.Password;
            // get user
            var user = _dbContext.Users.SingleOrDefault(u => u.Name == username);
            if (user == null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User not exists."));
            if (user.Status != UserStatus.Active)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active."));
            if (PasswordHasher.VerifyHashedPassword(user.Password, password))
            {
                // get token
                accessToken = JwtUtil.GenerateAccessToken(user.Id);
                refreshToken = JwtUtil.GenerateRefreshToken(user.Id);
                // set old refresh tokens to revoked
                if (request.DeviceId != null)
                {
                    var oldRefreshTokens = _dbContext.RefreshTokens
                        .Where(x => x.UserId == user.Id
                            && x.DeviceId == request.DeviceId.Id
                            && x.Status == Common.Models.TokenStatus.Normal);
                    foreach (var oldRefreshToken in oldRefreshTokens)
                    {
                        oldRefreshToken.Status = Common.Models.TokenStatus.Revoked;
                    }
                }
                _dbContext.RefreshTokens.Add(new Common.Models.RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.Id,
                    DeviceId = request.DeviceId?.Id
                });
                _dbContext.SaveChanges();
            }
            else
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Username and password not match."));
            }
            return Task.FromResult(new GetTokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
