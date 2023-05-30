using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Utils
{
    public static class UserUtil
    {
        public static UserType GetUserTypeFromJwt(ServerCallContext context, ApplicationDbContext db)
        {
            long internalId = JwtUtil.GetInternalIdFromJwt(context);
            var user = db.Users.Single(x => x.Id == internalId);
            return user.Type;
        }
    }
}
