//using Grpc.Core;
//using Librarian.Common.Utils;
//using Microsoft.AspNetCore.Authorization;
//using TuiHub.Protos.Librarian.Sephirah.V1;

//namespace Librarian.Sephirah.Services
//{
//    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
//    {
//        [Authorize]
//        public override Task<UnLinkAccountResponse> UnLinkAccount(UnLinkAccountRequest request, ServerCallContext context)
//        {
//            var userId = context.GetInternalIdFromHeader();
//            if (request == null)
//            {
//                throw new RpcException(new Status(StatusCode.InvalidArgument, "Account ID is required."));
//            }
//            var userDb = _dbContext.Users.Single(u => u.Id == userId);
//            var accountDb = userDb.Accounts.SingleOrDefault(a => a.Platform == request.Platform
//                && a.PlatformAccountId == request.PlatformAccountId);
//            if (accountDb == null)
//            {
//                throw new RpcException(new Status(StatusCode.InvalidArgument, "Account not existed."));
//            }
//            userDb.Accounts.Remove(accountDb);
//            _dbContext.SaveChanges();
//            return Task.FromResult(new UnLinkAccountResponse());
//        }
//    }
//}

