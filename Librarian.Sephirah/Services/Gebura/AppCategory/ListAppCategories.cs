using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    [Authorize]
    public override Task<ListAppCategoriesResponse> ListAppCategories(ListAppCategoriesRequest request,
        ServerCallContext context)
    {
        var userId = context.GetInternalIdFromHeader();
        var appCategories = _dbContext.AppCategories
            .Where(x => x.UserId == userId)
            .Include(x => x.User)
            .Include(x => x.Apps);
        return Task.FromResult(new ListAppCategoriesResponse
        {
            AppCategories = { appCategories.Select(x => x.ToPb()) }
        });
    }
}