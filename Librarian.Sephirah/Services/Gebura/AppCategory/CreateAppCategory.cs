using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<CreateAppCategoryResponse> CreateAppCategory(CreateAppCategoryRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var internalId = _idGenerator.CreateId();
            var appCategory = new Common.Models.Db.AppCategory(internalId, userId, request.AppCategory);
            _dbContext.AppCategories.Add(appCategory);
            _dbContext.SaveChanges();
            var ret = new CreateAppCategoryResponse
            {
                Id = new InternalID { Id = internalId }
            };
            return Task.FromResult(ret);
        }
    }
}
