using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<ListAppCategoriesResponse> ListAppCategories(ListAppCategoriesRequest request, ServerCallContext context)
        {
            var userId = JwtUtil.GetInternalIdFromJwt(context);
            var appCategories = _dbContext.AppCategories.Where(x => x.UserId == userId);
            var ret = new ListAppCategoriesResponse();
            ret.AppCategories.Add(appCategories.Select(x => x.ToProtoAppCategory()));
            return Task.FromResult(ret);
        }
    }
}
