using Grpc.Core;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize(AuthenticationSchemes = "RefreshToken")]
        public override Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            string accessTokenNew, refreshTokenNew;
            var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
            var internalId = JwtUtil.GetInternalIdFromToken(token);
            // get user
            var user = _dbContext.Users.Single(x => x.Id == internalId);
            if (user.Status != UserStatus.Active)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active."));
            // get new token
            accessTokenNew = JwtUtil.GenerateAccessToken(internalId);
            refreshTokenNew = JwtUtil.GenerateRefreshToken(internalId);
            return Task.FromResult(new RefreshTokenResponse
            {
                AccessToken = accessTokenNew,
                RefreshToken = refreshTokenNew
            });
        }
    }
}
