﻿using Grpc.Core;
using Librarian.Sephirah.Utils;
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
            using var db = new ApplicationDbContext();
            // create app package
            var internalId = IdUtil.NewId();
            var appPackage = new Models.AppPackage(internalId, request.AppPackage);
            var app = db.Apps.Single(x => x.Id == appPackage.SourceAppId);
            app.AppPackages.Add(appPackage);
            db.SaveChanges();
            return Task.FromResult(new CreateAppPackageResponse
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = internalId }
            });
        }
    }
}