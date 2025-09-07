using Grpc.Core;
using Librarian.Common.Constants;
using Librarian.Common.Converters;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    [Authorize]
    [EnableRateLimiting("bcrypt_fixed")]
    public override Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
    {
        var userIdFromJwt = context.GetInternalIdFromHeader();
        var userReq = request.User;
        // verify user type if request user id is not same as jwt user id
        if (userReq.Id.Id != userIdFromJwt)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext, "You don't have permission to update this user.");
        var user = _dbContext.Users.SingleOrDefault(x => x.Id == userReq.Id.Id);
        if (user == null) throw new RpcException(new Status(StatusCode.InvalidArgument, "User not exists."));
        if (!string.IsNullOrEmpty(userReq.Username)) user.Name = userReq.Username;
        if (!string.IsNullOrEmpty(userReq.Password)) user.Password = PasswordHasher.HashPassword(userReq.Password);
        if (userReq.Type != UserType.Unspecified) user.Type = userReq.Type.ToString().ToEnum<Enums.UserType>();
        if (userReq.Status != UserStatus.Unspecified)
            user.Status = userReq.Status.ToString().ToEnum<Enums.UserStatus>();
        user.UpdatedAt = DateTime.UtcNow;
        _dbContext.SaveChanges();
        return Task.FromResult(new UpdateUserResponse());
    }
}