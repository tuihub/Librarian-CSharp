using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Utils
{
    public static class UserUtil
    {
        public static UserType GetUserTypeFromToken(ServerCallContext context, TestDbContext db)
        {
            var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
            var internalIdFromToken = JwtUtil.GetInternalIdFromToken(token);
            var userFromToken = db.Users.Single(x => x.InternalId == internalIdFromToken);
            return userFromToken.Type;
        }
    }
}
