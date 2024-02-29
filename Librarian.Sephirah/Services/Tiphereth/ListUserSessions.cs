using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<ListUserSessionsResponse> ListUserSessions(ListUserSessionsRequest request, ServerCallContext context)
        {
            var userId = context.GetInternalIdFromHeader();
            var sessions = _dbContext.Sessions
                .Where(s => s.ExpiredAt > DateTime.UtcNow
                    && s.UserId == userId
                    && s.Status == Common.Models.TokenStatus.Normal)
                .Include(x => x.Device);
            var response = new ListUserSessionsResponse();
            response.Sessions.AddRange(sessions.Select(x => x.ToProtoUserSession()));
            return Task.FromResult(response);
        }
    }
}
