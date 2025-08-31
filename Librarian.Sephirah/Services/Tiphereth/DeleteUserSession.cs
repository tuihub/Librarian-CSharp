using Grpc.Core;
using Librarian.Common.Models.Db;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

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
                    && x.Status == TokenStatus.Normal);
            if (session == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Session not found"));
            }
            session.Status = TokenStatus.Deleted;
            session.UpdatedAt = DateTime.UtcNow;
            _dbContext.SaveChanges();
            return Task.FromResult(new DeleteUserSessionResponse());
        }
    }
}
