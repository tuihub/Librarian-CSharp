using Grpc.Core;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.RateLimiting;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // TODO: reduce token expiration time when DeviceId is not present
        [EnableRateLimiting("bcrypt_fixed")]
        public override Task<GetTokenResponse> GetToken(GetTokenRequest request, ServerCallContext context)
        {
            string accessToken, refreshToken;
            var username = request.Username;
            var password = request.Password;
            var deviceId = request.DeviceId?.Id;
            // get user
            var user = _dbContext.Users.SingleOrDefault(u => u.Name == username);
            if (user == null)
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User not exists."));
            }
            if (user.Status != UserStatus.Active)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active."));
            }
            if (PasswordHasher.VerifyHashedPassword(user.Password, password))
            {
                // get token
                accessToken = JwtUtil.GenerateAccessToken(user.Id);
                refreshToken = JwtUtil.GenerateRefreshToken(user.Id);
                // set old sessions (refresh tokens) to revoked
                if (deviceId != null)
                {
                    var oldSessions = _dbContext.Sessions
                        .Where(x => x.ExpiredAt > DateTime.UtcNow
                            && x.UserId == user.Id
                            && x.DeviceId == deviceId
                            && x.Status == TokenStatus.Normal);
                    foreach (var session in oldSessions)
                    {
                        session.Status = TokenStatus.Revoked;
                        session.UpdatedAt = DateTime.UtcNow;
                    }
                    _dbContext.Sessions.Add(new Common.Models.Db.Session
                    {
                        InternalId = _idGenerator.CreateId(),
                        TokenJti = refreshToken.GetJtiFromJwtToken(),
                        UserId = user.Id,
                        DeviceId = (long)deviceId,
                        ExpiredAt = JwtUtil.GetTokenExpireTime(refreshToken)
                    });
                    _dbContext.SaveChanges();
                }
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
