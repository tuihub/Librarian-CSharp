using Grpc.Core;
using Librarian.Common.Constants;

namespace Librarian.Common.Utils;

public static class UserUtil
{
    public static Enums.UserType GetUserTypeFromJwt(ServerCallContext context, ApplicationDbContext db)
    {
        var internalId = context.GetInternalIdFromHeader();
        var user = db.Users.Single(x => x.Id == internalId);
        return user.Type;
    }

    public static void VerifyUserAdminAndThrow(ServerCallContext context, ApplicationDbContext db, string? exMsg = null)
    {
        if (GetUserTypeFromJwt(context, db) != Enums.UserType.Admin)
            throw new RpcException(new Status(StatusCode.PermissionDenied, exMsg ?? "Access Deined."));
    }
}