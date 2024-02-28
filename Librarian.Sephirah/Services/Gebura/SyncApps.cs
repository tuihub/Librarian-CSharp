﻿using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // TODO: impl WaitData, rate limit
        [Authorize]
        public override async Task<SyncAppsResponse> SyncApps(SyncAppsRequest request, ServerCallContext context)
        {
            // get request param
            var protoAppIds = request.AppIds;
            foreach (var protoAppId in protoAppIds)
            {
                // internal app
                if (protoAppId.Internal)
                {
                    var result = long.TryParse(protoAppId.SourceAppId, out var appId);
                    if (!result)
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid AppId."));
                    }
                    var app = _dbContext.AppInfos.Include(x => x.ChildAppInfos)
                                             .SingleOrDefault(x => x.Id == appId && x.Source == Common.Constants.Proto.AppSourceInternal);
                    if (app == null)
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
                    }
                    foreach (var childApp in app.ChildAppInfos)
                    {
                        _pullMetadataService.AddPullApp(childApp.Id);
                    }
                }
                else
                {
                    var appSource = protoAppId.Source;
                    var appId = protoAppId.SourceAppId;
                    if (appSource == Common.Constants.Proto.AppSourceUnspecified ||
                        appSource == Common.Constants.Proto.AppSourceInternal)
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid AppSource."));
                    }
                    if (string.IsNullOrWhiteSpace(appId))
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid AppId."));
                    }

                    var app = _dbContext.AppInfos.SingleOrDefault(x => x.Source == appSource && x.SourceAppId == appId);
                    // external app not exists
                    if (app == null)
                    {
                        // create new internal app
                        var newInternalApp = new Common.Models.AppInfo
                        {
                            Id = _idGenerator.CreateId(),
                            Source = Common.Constants.Proto.AppSourceInternal,
                            Name = $"{protoAppId.Source}_{protoAppId.SourceAppId}",
                            Type = TuiHub.Protos.Librarian.V1.AppType.Game
                        };
                        var newExternalApp = new Common.Models.AppInfo
                        {
                            Id = _idGenerator.CreateId(),
                            Source = appSource,
                            SourceAppId = appId,
                            Name = string.Empty,
                            Type = TuiHub.Protos.Librarian.V1.AppType.Game,
                            ParentAppInfo = newInternalApp
                        };
                        _dbContext.AppInfos.Add(newInternalApp);
                        _dbContext.AppInfos.Add(newExternalApp);
                        await _dbContext.SaveChangesAsync();
                        _pullMetadataService.AddPullApp(newExternalApp.Id, true);
                    }
                    // external app exists
                    else
                    {
                        _pullMetadataService.AddPullApp(app.Id);
                    }
                }
            }
            return new SyncAppsResponse();
        }
    }
}
