using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<UnLinkAccountResponse> UnLinkAccount(UnLinkAccountRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var accountId = request.AccountId;
            if (accountId == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Account ID is required."));
            }
            var userDb = _dbContext.Users.Single(u => u.Id == userId);
            var accountDb = userDb.Accounts.SingleOrDefault(a => a.Platform == accountId.Platform
                && a.PlatformAccountId == accountId.PlatformAccountId);
            if (accountDb == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Account not existed."));
            }
            userDb.Accounts.Remove(accountDb);
            _dbContext.SaveChanges();
            return Task.FromResult(new UnLinkAccountResponse());
        }
    }
}
