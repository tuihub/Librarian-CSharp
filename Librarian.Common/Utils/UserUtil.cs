using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Common.Utils
{
    public static class UserUtil
    {
        public static UserType GetUserTypeFromJwt(ServerCallContext context, ApplicationDbContext db)
        {
            long internalId = JwtUtil.GetInternalIdFromJwt(context);
            var user = db.Users.Single(x => x.Id == internalId);
            return user.Type;
        }

        public static void VerifyUserAdminAndThrow(ServerCallContext context, ApplicationDbContext db)
        {
            if (GetUserTypeFromJwt(context, db) != UserType.Admin)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            }
        }
    }
}
