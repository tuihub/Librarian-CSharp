using Grpc.Core;
using Librarian.Common.Utils;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<ListUsersResponse> ListUsers(ListUsersRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            IQueryable<Common.Models.User> usersDb = _dbContext.Users;
            // exclude current user
            usersDb = usersDb.Where(u => u.Id != context.GetInternalIdFromHeader());
            if (request.TypeFilter.Count > 0)
            {
                usersDb = usersDb.Where(u => request.TypeFilter.Contains(u.Type));
            }
            if (request.StatusFilter.Count > 0)
            {
                usersDb = usersDb.Where(u => request.StatusFilter.Contains(u.Status));
            }
            usersDb = usersDb.ApplyPagingRequest(request.Paging);
            var users = usersDb.ToList();
            var response = new ListUsersResponse();
            response.Users.AddRange(users.Select(u => u.ToProtoUser()));
            response.Paging = new PagingResponse { TotalSize = users.Count };
            return Task.FromResult(response);
        }
    }
}
