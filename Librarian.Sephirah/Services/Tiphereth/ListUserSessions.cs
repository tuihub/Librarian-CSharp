using Grpc.Core;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

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
                    && s.Status == TokenStatus.Normal)
                .Include(x => x.Device);
            var response = new ListUserSessionsResponse();
            response.Sessions.AddRange(sessions.Select(x => x.ToPb()));
            return Task.FromResult(response);
        }
    }
}
