using Grpc.Core;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>
        /// AppInfos without details
        /// </returns>
        [Authorize]
        public override Task<GetPurchasedAppInfosResponse> GetPurchasedAppInfos(GetPurchasedAppInfosRequest request, ServerCallContext context)
        {
            var appInfoSource = request.Source;
            var userId = context.GetInternalIdFromHeader();
            // foreach for IEnumerable is not by reference
            // https://stackoverflow.com/questions/43055464/c-sharp-foreach-not-reference-to-original-objects-but-copies
            var appInfos = _dbContext.Users
                .Where(x => x.Id == userId)
                .Include(x => x.AppInfos)
                .ThenInclude(x => x.ChildAppInfos)
                .Single(x => x.Id == userId)
                .AppInfos
                .ToList();
            if (string.IsNullOrEmpty(appInfoSource))
            {
                appInfos = appInfos.Where(x => x.Source == appInfoSource).ToList();
            }
            // construct return value
            var response = new GetPurchasedAppInfosResponse();
            response.AppInfos.AddRange(appInfos.Select(x => x.Flatten().ToProtoAppInfoMixed()));
            return Task.FromResult(response);
        }
    }
}
