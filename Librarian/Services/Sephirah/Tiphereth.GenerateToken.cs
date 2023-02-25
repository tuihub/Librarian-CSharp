using Grpc.Core;
using Librarian.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Services.Sephirah
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request, ServerCallContext context)
        {
            string refreshToken;
            try
            {
                using var db = new TestDbContext();
                // verify user type(admin)
                if (UserUtil.GetUserTypeFromToken(context, db) != UserType.Admin)
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
                // generate token
                var internalId = request.Id.Id;
                var user = db.Users.SingleOrDefault(x => x.InternalId == internalId);
                if (user == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "User not exists."));
                refreshToken = JwtUtil.GenerateRefreshToken(internalId);
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, ex.Message));
            }
            return Task.FromResult(new GenerateTokenResponse()
            {
                RefreshToken = refreshToken
            });
        }
    }
}
