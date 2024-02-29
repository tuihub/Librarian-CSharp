using Grpc.Core;
using Librarian.Common.Models;
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
            var internalId = context.GetInternalIdFromHeader();
            var deviceId = request.DeviceId?.Id;
            // get user
            var user = _dbContext.Users.Single(x => x.Id == internalId);
            if (user.Status != UserStatus.Active)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active."));
            }
            // get db refresh token only when DeviceId is present
            if (deviceId != null)
            {
                var token = context.GetBearerToken();
                var refreshTokenDb = _dbContext.RefreshTokens
                    .SingleOrDefault(x => x.Token == token
                        && x.UserId == internalId
                        && x.DeviceId == deviceId
                        && x.Status == Common.Models.TokenStatus.Normal);
                if (refreshTokenDb == null)
                {
                    throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid refresh token."));
                }
                refreshTokenDb.Status = Common.Models.TokenStatus.Used;
            }
            // get new token
            accessTokenNew = JwtUtil.GenerateAccessToken(internalId);
            refreshTokenNew = JwtUtil.GenerateRefreshToken(internalId);
            if (deviceId != null)
            {
                _dbContext.RefreshTokens.Add(new Common.Models.RefreshToken
                {
                    Token = refreshTokenNew,
                    UserId = user.Id,
                    DeviceId = (long)deviceId
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
