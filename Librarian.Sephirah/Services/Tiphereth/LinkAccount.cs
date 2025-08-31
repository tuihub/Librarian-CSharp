//using Grpc.Core;
//using Librarian.Common.Utils;
//using Microsoft.AspNetCore.Authorization;
//using TuiHub.Protos.Librarian.Sephirah.V1;
//using TuiHub.Protos.Librarian.V1;

//namespace Librarian.Sephirah.Services
//{
//    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
//    {
//        [Authorize]
//        public override Task<LinkAccountResponse> LinkAccount(LinkAccountRequest request, ServerCallContext context)
//        {
//            var userId = context.GetInternalIdFromHeader();
//            if (request == null)
//            {
//                throw new RpcException(new Status(StatusCode.InvalidArgument, "Account ID is required."));
//            }
//            var userDb = _dbContext.Users.Single(u => u.Id == userId);
//            if (userDb.Accounts.Any(a => a.Platform == request.Platform && a.PlatformAccountId == request.PlatformAccountId))
//            {
//                throw new RpcException(new Status(StatusCode.InvalidArgument, "Account already linked."));
//            }
//            var accountInternalId = _idGenerator.CreateId();
//            userDb.Accounts.Add(new Common.Models.Db.Account
//            {
//                Id = accountInternalId,
//                Platform = request.Platform,
//                PlatformAccountId = request.PlatformAccountId
//            });
//            _dbContext.SaveChanges();
//            return Task.FromResult(new LinkAccountResponse
//            {
//                AccountId = new InternalID { Id = accountInternalId }
//            });
//        }
//    }
//}
