using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Utils
{
    public static class UserUtil
    {
        public static UserType GetUserTypeFromToken(ServerCallContext context, ApplicationDbContext db)
        {
            var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
            var internalIdFromToken = JwtUtil.GetInternalIdFromToken(token);
            var userFromToken = db.Users.Single(x => x.InternalId == internalIdFromToken);
            return userFromToken.Type;
        }
    }
}
