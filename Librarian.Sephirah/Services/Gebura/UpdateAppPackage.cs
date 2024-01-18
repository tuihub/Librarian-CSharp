using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<UpdateAppPackageResponse> UpdateAppPackage(UpdateAppPackageRequest request, ServerCallContext context)
        {
            // verify user type(admin)
            UserUtil.VerifyUserAdminAndThrow(context, _dbContext);
            // check AppPackage exists
            var appPackageReq = request.AppPackage;
            var appPackage = _dbContext.AppPackages.SingleOrDefault(x => x.Id == appPackageReq.Id.Id);
            if (appPackage == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppPackage not exists."));
            }
            // update AppPackage
            appPackage.Source = appPackageReq.Source;
            appPackage.SourceAppId = appPackageReq.SourceId.Id;
            appPackage.Name = appPackageReq.Name;
            appPackage.Description = appPackageReq.Description;
            appPackage.IsPublic = appPackageReq.Public;
            appPackage.AppPackageBinary = appPackage.AppPackageBinary;
            appPackage.UpdatedAt = DateTime.Now;
            _dbContext.SaveChanges();
            return Task.FromResult(new UpdateAppPackageResponse());
        }
    }
}
