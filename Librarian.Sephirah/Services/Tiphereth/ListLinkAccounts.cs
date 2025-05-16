using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // TODO: add support fot listing other user's linked accounts
        [Authorize]
        public override Task<ListLinkAccountsResponse> ListLinkAccounts(ListLinkAccountsRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var user = _dbContext.Users.Single(u => u.Id == userId);
            var accounts = user.Accounts;
            var response = new ListLinkAccountsResponse();
            response.Accounts.AddRange(accounts.Select(a => a.ToPB()));
            return Task.FromResult(response);
        }
    }
}
