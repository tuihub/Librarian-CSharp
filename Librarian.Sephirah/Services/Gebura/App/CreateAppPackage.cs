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
        public override Task<CreateAppPackageResponse> CreateAppPackage(CreateAppPackageRequest request, ServerCallContext context)
        {
            // create app package
            var internalId = _idGenerator.CreateId();
            var appPackage = new Common.Models.AppPackage(internalId, request.AppPackage);
            var app = _dbContext.AppInfos.Single(x => x.Id == appPackage.SourceAppId);
            app.AppPackages.Add(appPackage);
            _dbContext.SaveChanges();
            return Task.FromResult(new CreateAppPackageResponse
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = internalId }
            });
        }
    }
}
