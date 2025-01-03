﻿using Grpc.Core;
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
        // TODO: add support fot listing other user's linked accounts
        [Authorize]
        public override Task<ListLinkAccountsResponse> ListLinkAccounts(ListLinkAccountsRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var user = _dbContext.Users.Single(u => u.Id == userId);
            var accounts = user.Accounts;
            var response = new ListLinkAccountsResponse();
            response.Accounts.AddRange(accounts.Select(a => a.ToProto()));
            return Task.FromResult(response);
        }
    }
}
