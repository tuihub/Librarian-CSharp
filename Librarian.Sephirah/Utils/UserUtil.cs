using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Utils
{
    public static class UserUtil
    {
        public static UserType GetUserTypeFromToken(ServerCallContext context, ApplicationDbContext db)
        {
            long internalIdFromToken = GetUserInternalIdFromToken(context);
            var userFromToken = db.Users.Single(x => x.Id == internalIdFromToken);
            return userFromToken.Type;
        }

        public static long GetUserInternalIdFromToken(ServerCallContext context)
        {
            var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
            var internalIdFromToken = JwtUtil.GetInternalIdFromToken(token);
            return internalIdFromToken;
        }
    }
}
