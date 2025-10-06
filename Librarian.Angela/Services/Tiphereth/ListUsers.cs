using Grpc.Core;
using Librarian.Sephirah.Angela;
using Librarian.Common.Constants;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Angela.Services;

public partial class AngelaService
{
    [Authorize(Policy = "AngelaAccess")]
    public override async Task<ListUsersResponse> ListUsers(ListUsersRequest request, ServerCallContext context)
    {
        // Verify that the user is an administrator
        if (context.GetBearerToken() != null)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

        // Query users
        IQueryable<Common.Models.Db.User> usersDb = _dbContext.Users;

        // Exclude current user
        // usersDb = usersDb.Where(u => u.Id != context.GetInternalIdFromHeader());

        // Filter by requested type and status
        if (request.TypeFilter.Count > 0)
        {
            var typeFilters = request.TypeFilter.Select(t => ConvertToDbUserType(t)).ToList();
            usersDb = usersDb.Where(u => typeFilters.Contains(u.Type));
        }

        if (request.StatusFilter.Count > 0)
        {
            var statusFilters = request.StatusFilter.Select(s => ConvertToDbUserStatus(s)).ToList();
            usersDb = usersDb.Where(u => statusFilters.Contains(u.Status));
        }

        // Apply paging
        usersDb = ApplyAngelaPagingRequest(usersDb, request.Paging);

        // Get final results
        var users = await usersDb.ToListAsync();

        // Build response
        var response = new ListUsersResponse
        {
            Paging = new PagingResponse { TotalSize = users.Count }
        };

        // Add users to response
        response.Users.AddRange(users.Select(u => ConvertToProtoUser(u)));

        return response;
    }

    private IQueryable<T> ApplyAngelaPagingRequest<T>(IQueryable<T> source, PagingRequest? pagingRequest)
    {
        if (pagingRequest == null) return source;

        var pageSize = pagingRequest.PageSize;
        var pageNum = pagingRequest.PageNum;
        return source.Skip((int)(pageSize * (pageNum - 1))).Take((int)pageSize);
    }

    private Enums.UserType ConvertToDbUserType(UserType protoType)
    {
        return protoType switch
        {
            UserType.Admin => Enums.UserType.Admin,
            UserType.Normal => Enums.UserType.Normal,
            _ => Enums.UserType.Normal
        };
    }

    private Enums.UserStatus ConvertToDbUserStatus(UserStatus protoStatus)
    {
        return protoStatus switch
        {
            UserStatus.Active => Enums.UserStatus.Active,
            UserStatus.Blocked => Enums.UserStatus.Blocked,
            _ => Enums.UserStatus.Active
        };
    }

    private User ConvertToProtoUser(Common.Models.Db.User dbUser)
    {
        return new User
        {
            Id = new InternalID { Id = dbUser.Id },
            Username = dbUser.Name,
            Type = ConvertToProtoUserType(dbUser.Type),
            Status = ConvertToProtoUserStatus(dbUser.Status)
        };
    }

    private UserType ConvertToProtoUserType(Enums.UserType dbType)
    {
        return dbType switch
        {
            Enums.UserType.Admin => UserType.Admin,
            Enums.UserType.Normal => UserType.Normal,
            _ => UserType.Normal
        };
    }

    private UserStatus ConvertToProtoUserStatus(Enums.UserStatus dbStatus)
    {
        return dbStatus switch
        {
            Enums.UserStatus.Active => UserStatus.Active,
            Enums.UserStatus.Blocked => UserStatus.Blocked,
            _ => UserStatus.Active
        };
    }
}