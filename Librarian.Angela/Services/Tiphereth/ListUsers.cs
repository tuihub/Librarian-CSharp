//using Grpc.Core;
//using Librarian.Common.Constants;
//using Librarian.Common.Converters;
//using Librarian.Common.Helpers;
//using Librarian.Common.Utils;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.EntityFrameworkCore;
//using TuiHub.Protos.Librarian.Sephirah.V1.Angela;
//using TuiHub.Protos.Librarian.V1;

//namespace Librarian.Angela.Services
//{
//    public partial class AngelaService
//    {
//        [Authorize]
//        public override async Task<ListUsersResponse> ListUsers(ListUsersRequest request, ServerCallContext context)
//        {
//            // Verify that the user is an administrator
//            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);

//            // Query users
//            IQueryable<Common.Models.Db.User> usersDb = _dbContext.Users;

//            // Exclude current user
//            usersDb = usersDb.Where(u => u.Id != context.GetInternalIdFromHeader());

//            // Filter by requested type and status
//            if (request.TypeFilter.Count > 0)
//            {
//                usersDb = usersDb.Where(u => request.TypeFilter.Select(t => t.ToString().ToEnum<Enums.UserType>()).Contains(u.Type));
//            }

//            if (request.StatusFilter.Count > 0)
//            {
//                usersDb = usersDb.Where(u => request.StatusFilter.Select(s => s.ToString().ToEnum<Enums.UserStatus>()).Contains(u.Status));
//            }

//            // Apply paging
//            usersDb = usersDb.ApplyPagingRequest(request.Paging);

//            // Get final results
//            var users = await usersDb.ToListAsync();

//            // Build response
//            var response = new ListUsersResponse
//            {
//                Paging = new PagingResponse { TotalSize = users.Count }
//            };

//            // Add users to response
//            response.Users.AddRange(users.Select(u => u.ToPb()));

//            return response;
//        }
//    }
//}
