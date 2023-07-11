using Grpc.Core;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<UpdateAppCategoryResponse> UpdateAppCategory(UpdateAppCategoryRequest request, ServerCallContext context)
        {
            var userId = JwtUtil.GetInternalIdFromJwt(context);
            var appCategory = _dbContext.AppCategories.SingleOrDefault(x => x.Id == request.AppCategory.Id.Id);
            if (appCategory == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppCategory not exists."));
            if (appCategory.UserId != userId)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
            appCategory.Name = request.AppCategory.Name;
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppCategoryResponse());
        }
    }
}
