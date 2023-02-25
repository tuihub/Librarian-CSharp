﻿using Grpc.Core;
using Librarian.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Services.Sephirah
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        [Authorize]
        public override Task<CreateAppResponse> CreateApp(CreateAppRequest request, ServerCallContext context)
        {
            long internalId;
            try
            {
                using var db = new TestDbContext();
                // verify user type(admin)
                if (UserUtil.GetUserTypeFromToken(context, db) != UserType.Admin)
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "Access Deined."));
                // create app
                internalId = IdUtil.NewId();
                if (request.App.Source != AppSource.Internal)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "AppSource must be APP_SOURCE_INTERNAL."));
                var app = new Models.App(internalId, request.App);
                db.Apps.Add(app);
                db.SaveChanges();
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Unavailable, ex.Message));
            }
            return Task.FromResult(new CreateAppResponse
            {
                Id = new TuiHub.Protos.Librarian.V1.InternalID { Id = internalId }
            });
        }
    }
}
