using Grpc.Core;
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
        public override async Task<SyncAppInfosResponse> SyncAppInfos(SyncAppInfosRequest request, ServerCallContext context)
        {
            // get request param
            var protoAppInfoIds = request.AppInfoIds;
            foreach (var protoAppInfoId in protoAppInfoIds)
            {
                // internal appInfo
                if (protoAppInfoId.Internal)
                {
                    var result = long.TryParse(protoAppInfoId.SourceAppId, out var appId);
                    if (!result)
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid AppInfoId."));
                    }
                    var appInfo = _dbContext.AppInfos.Include(x => x.ChildAppInfos)
                                             .SingleOrDefault(x => x.Id == appId && x.Source == Common.Constants.Proto.AppInfoSourceInternal);
                    if (appInfo == null)
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfo not exists."));
                    }
                    foreach (var childAppInfo in appInfo.ChildAppInfos)
                    {
                        _pullMetadataService.AddPullAppInfo(childAppInfo.Id);
                    }
                }
                else
                {
                    var appInfoSource = protoAppInfoId.Source;
                    var appInfoId = protoAppInfoId.SourceAppId;
                    if (appInfoSource == Common.Constants.Proto.AppInfoSourceUnspecified ||
                        appInfoSource == Common.Constants.Proto.AppInfoSourceInternal)
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid AppInfoSource."));
                    }
                    if (string.IsNullOrWhiteSpace(appInfoId))
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid AppInfoId."));
                    }

                    var appInfo = _dbContext.AppInfos.SingleOrDefault(x => x.Source == appInfoSource && x.SourceAppId == appInfoId);
                    // external appInfo not exists
                    if (appInfo == null)
                    {
                        // create new internal appInfo
                        var newInternalAppInfo = new Common.Models.AppInfo
                        {
                            Id = _idGenerator.CreateId(),
                            Source = Common.Constants.Proto.AppInfoSourceInternal,
                            Name = $"{protoAppInfoId.Source}_{protoAppInfoId.SourceAppId}",
                            Type = TuiHub.Protos.Librarian.V1.AppType.Game
                        };
                        var newExternalAppInfo = new Common.Models.AppInfo
                        {
                            Id = _idGenerator.CreateId(),
                            Source = appInfoSource,
                            SourceAppId = appInfoId,
                            Name = string.Empty,
                            Type = TuiHub.Protos.Librarian.V1.AppType.Game,
                            ParentAppInfo = newInternalAppInfo
                        };
                        _dbContext.AppInfos.Add(newInternalAppInfo);
                        _dbContext.AppInfos.Add(newExternalAppInfo);
                        await _dbContext.SaveChangesAsync();
                        _pullMetadataService.AddPullAppInfo(newExternalAppInfo.Id, true);
                    }
                    // external appInfo exists
                    else
                    {
                        _pullMetadataService.AddPullAppInfo(appInfo.Id);
                    }
                }
            }
            return new SyncAppInfosResponse();
        }
    }
}
