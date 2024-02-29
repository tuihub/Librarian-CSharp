using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
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
        public override Task<DeleteUserSessionResponse> DeleteUserSession(DeleteUserSessionRequest request, ServerCallContext context)
        {
            var sessionId = request.SessionId.Id;
            var session = _dbContext.Sessions
                .SingleOrDefault(x => x.Id == sessionId
                    && x.Status == Common.Models.TokenStatus.Normal);
            if (session == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Session not found"));
            }
            session.Status = Common.Models.TokenStatus.Deleted;
            session.UpdatedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            return Task.FromResult(new DeleteUserSessionResponse());
        }
    }
}
