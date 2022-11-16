using Grpc.Core;
using Librarian.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Services.Sephirah
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize(AuthenticationSchemes = "RefreshToken")]
        public override Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request, ServerCallContext context)
        {
            string accessTokenNew, refreshTokenNew;
            try
            {
                using var db = new TestDbContext();
                var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
                var internalId = JwtUtil.GetInternalIdFromToken(token);
                // get user
                var user = db.Users.Single(x => x.InternalId == internalId);
                if (user.Status != UserStatus.Active)
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "User not active."));
                // get new token
                accessTokenNew = JwtUtil.GenerateAccessToken(internalId);
                refreshTokenNew = JwtUtil.GenerateRefreshToken(internalId);
            }
            catch (RpcException)
            {
                throw;
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
