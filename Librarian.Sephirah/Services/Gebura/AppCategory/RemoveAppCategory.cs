using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<DeleteAppCategoryResponse> DeleteAppCategory(DeleteAppCategoryRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var appCategory = _dbContext.AppCategories
                .Where(x => x.Id == request.Id.Id)
                .Include(x => x.User)
                .Include(x => x.Apps)
                .SingleOrDefault(x => x.Id == request.Id.Id);
            if (appCategory == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppCategory not exists."));
            }
            if (appCategory.UserId != userId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "AppCategory is not owned by user."));
            }
            appCategory.Apps.Clear();
            _dbContext.SaveChanges();
            _dbContext.AppCategories.Remove(appCategory);
            _dbContext.SaveChanges();
            return Task.FromResult(new DeleteAppCategoryResponse());
        }
    }
}