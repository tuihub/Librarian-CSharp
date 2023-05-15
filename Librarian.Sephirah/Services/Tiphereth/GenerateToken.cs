using Grpc.Core;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<GenerateTokenResponse> GenerateToken(GenerateTokenRequest request, ServerCallContext context)
        {
            string refreshToken;
            using var db = new ApplicationDbContext();
            // verify user type(admin)
            if (UserUtil.GetUserTypeFromToken(context, db) != UserType.Admin)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            // generate token
            var internalId = request.Id.Id;
            var user = db.Users.SingleOrDefault(x => x.Id == internalId);
            if (user == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "User not exists."));
            refreshToken = JwtUtil.GenerateRefreshToken(internalId);
            return Task.FromResult(new GenerateTokenResponse()
            {
                RefreshToken = refreshToken
            });
        }
    }
}
