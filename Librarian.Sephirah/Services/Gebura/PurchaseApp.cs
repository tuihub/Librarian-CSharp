using Grpc.Core;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<PurchaseAppResponse> PurchaseApp(PurchaseAppRequest request, ServerCallContext context)
        {
            using var db = new TestDbContext();
            var appId = request.AppId.Id;
            var app = db.Apps.SingleOrDefault(x => x.InternalId == appId);
            if (app == null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
            if (app.Source != AppSource.Internal)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppSource must be APP_SOURCE_INTERNAL."));
            var token = context.RequestHeaders.Single(x => x.Key == "authorization").Value;
            var user = db.Users.Single(x => x.InternalId == JwtUtil.GetInternalIdFromToken(token));
            user.Apps.Add(app);
            db.SaveChanges();
            return Task.FromResult(new PurchaseAppResponse());
        }
    }
}
