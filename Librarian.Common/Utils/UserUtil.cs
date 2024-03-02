using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Common.Utils
{
    public static class UserUtil
    {
        public static UserType GetUserTypeFromJwt(ServerCallContext context, ApplicationDbContext db)
        {
            long internalId = context.GetInternalIdFromHeader();
            var user = db.Users.Single(x => x.Id == internalId);
            return user.Type;
        }

        public static void VerifyUserAdminAndThrow(ServerCallContext context, ApplicationDbContext db, string? exMsg = null)
        {
            if (GetUserTypeFromJwt(context, db) != UserType.Admin)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, exMsg ?? "Access Deined."));
            }
        }
    }
}
