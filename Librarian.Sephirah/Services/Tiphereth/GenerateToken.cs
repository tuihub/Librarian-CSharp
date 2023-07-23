using Grpc.Core;
using Librarian.Common.Utils;
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
            // verify user type(admin)
            if (UserUtil.GetUserTypeFromJwt(context, _dbContext) != UserType.Admin)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            // generate token
            var internalId = request.Id.Id;
            var user = _dbContext.Users.SingleOrDefault(x => x.Id == internalId);
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
