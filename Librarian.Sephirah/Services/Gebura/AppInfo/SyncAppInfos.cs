using Grpc.Core;
using Librarian.Common.Models.Db;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
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
                    var appInfo = _dbContext.AppInfos
                        .Where(x => x.Id == appId && x.Source == Common.Constants.Proto.AppInfoSourceInternal)
                        .Include(x => x.ChildAppInfos)
                        .SingleOrDefault(x => x.Id == appId && x.Source == Common.Constants.Proto.AppInfoSourceInternal);
                    if (appInfo == null)
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfo not exists."));
                    }
                    foreach (var childAppInfo in appInfo.ChildAppInfos)
                    {
                        if (!string.IsNullOrWhiteSpace(childAppInfo.SourceAppId))
                        {
                            _messageQueueService.PublishMessage(childAppInfo.Source, JsonSerializer.Serialize(new AppIdMQ
                            {
                                AppId = childAppInfo.SourceAppId,
                                UpdateInternalAppInfoName = false
                            }));
                        }
                    }
                }
                else
                {
                    var source = protoAppInfoId.Source;
                    var sourceAppId = protoAppInfoId.SourceAppId;
                    if (source == Common.Constants.Proto.AppInfoSourceUnspecified ||
                        source == Common.Constants.Proto.AppInfoSourceInternal)
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid AppInfoSource."));
                    }
                    if (string.IsNullOrWhiteSpace(sourceAppId))
                    {
                        throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid AppInfoId."));
                    }

                    var appInfo = _dbContext.AppInfos.SingleOrDefault(x => x.Source == source && x.SourceAppId == sourceAppId);
                    // external appInfo not exists
                    if (appInfo == null)
                    {
                        // create new internal appInfo
                        var newInternalAppInfo = new Common.Models.Db.AppInfo
                        {
                            Id = _idGenerator.CreateId(),
                            Source = Common.Constants.Proto.AppInfoSourceInternal,
                            Name = $"{protoAppInfoId.Source}_{protoAppInfoId.SourceAppId}",
                            Type = TuiHub.Protos.Librarian.V1.AppType.Game
                        };
                        var newExternalAppInfo = new Common.Models.Db.AppInfo
                        {
                            Id = _idGenerator.CreateId(),
                            Source = source,
                            SourceAppId = sourceAppId,
                            Name = string.Empty,
                            Type = TuiHub.Protos.Librarian.V1.AppType.Game,
                            ParentAppInfo = newInternalAppInfo
                        };
                        _dbContext.AppInfos.Add(newInternalAppInfo);
                        _dbContext.AppInfos.Add(newExternalAppInfo);
                        await _dbContext.SaveChangesAsync();
                        _messageQueueService.PublishMessage(source, JsonSerializer.Serialize(new AppIdMQ
                        {
                            AppId = sourceAppId,
                            UpdateInternalAppInfoName = true
                        }));
                    }
                    // external appInfo exists
                    else
                    {
                        _messageQueueService.PublishMessage(source, JsonSerializer.Serialize(new AppIdMQ
                        {
                            AppId = sourceAppId,
                            UpdateInternalAppInfoName = false
                        }));
                    }
                }
            }
            return new SyncAppInfosResponse();
        }
    }
}
