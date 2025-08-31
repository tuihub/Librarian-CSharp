using Grpc.Core;
using Librarian.Common.Constants;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // TODO: reduce token expiration time when DeviceId is not present
        [Authorize(AuthenticationSchemes = "RefreshToken")]
        public override Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            string accessTokenNew, refreshTokenNew;
            Session? oldSession = null;
            var internalId = context.GetInternalIdFromHeader();
            var deviceId = request.DeviceId?.Id;
            // get user
            var user = _dbContext.Users.Single(x => x.Id == internalId);
            if (user.Status != Enums.UserStatus.Active)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active."));
            }
            // get db refresh token only when DeviceId is present
            if (deviceId != null)
            {
                var token = context.GetBearerToken();
                oldSession = _dbContext.Sessions
                    .SingleOrDefault(x => x.TokenJti == token!.GetJtiFromJwtToken()
                        && x.ExpiredAt > DateTime.UtcNow
                        && x.UserId == internalId
                        && x.DeviceId == deviceId
                        && x.Status == TokenStatus.Normal);
                if (oldSession == null)
                {
                    throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid refresh token."));
                }
                oldSession.Status = TokenStatus.Used;
                oldSession.UpdatedAt = DateTime.UtcNow;
            }
            // get new token
            accessTokenNew = JwtUtil.GenerateAccessToken(internalId);
            refreshTokenNew = JwtUtil.GenerateRefreshToken(internalId);
            if (deviceId != null)
            {
                _dbContext.Sessions.Add(new Common.Models.Db.Session
                {
                    InternalId = oldSession!.InternalId,
                    TokenJti = refreshTokenNew.GetJtiFromJwtToken(),
                    UserId = user.Id,
                    DeviceId = (long)deviceId,
                    CreatedAt = oldSession.CreatedAt,
                    ExpiredAt = JwtUtil.GetTokenExpireTime(refreshTokenNew)
                });
                // save to db only when DeviceId is present
                _dbContext.SaveChanges();
            }
            // return new tokens
            return Task.FromResult(new RefreshTokenResponse
            {
                AccessToken = accessTokenNew,
                RefreshToken = refreshTokenNew
            });
        }
    }
}
