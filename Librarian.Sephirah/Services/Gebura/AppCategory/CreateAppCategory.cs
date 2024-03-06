using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        /// <summary>
        /// Create a new app category, ignore any appInfo and app relation
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override Task<CreateAppCategoryResponse> CreateAppCategory(CreateAppCategoryRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var internalId = _idGenerator.CreateId();
            var appCategory = new Common.Models.AppCategory(internalId, userId, request.AppCategory);
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
