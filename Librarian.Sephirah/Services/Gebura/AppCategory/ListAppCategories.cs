using Grpc.Core;
using Librarian.Common.Models;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<ListAppCategoriesResponse> ListAppCategories(ListAppCategoriesRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var appCategories = _dbContext.AppCategories
                .Include(x => x.User)
                .Where(x => x.UserId == userId)
                .Include(x => x.AppInfos)
                .Include(x => x.Apps);
            return Task.FromResult(new ListAppCategoriesResponse
            {
                AppCategories = { appCategories.Select(x => x.ToProtoAppCategory()) }
            });
        }
    }
}
