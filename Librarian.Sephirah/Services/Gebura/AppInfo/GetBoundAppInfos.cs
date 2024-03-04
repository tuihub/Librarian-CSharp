using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>
        /// <para>Get all associated AppInfos when requested AppInfoSource is INTERNAL</para>
        /// <para>Get the exact AppInfo and its INTERNAL ParentAppInfo when requested AppInfoSource IS NOT INTERNAL</para>
        /// </returns>
        [Authorize]
        public override Task<GetBoundAppInfosResponse> GetBoundAppInfos(GetBoundAppInfosRequest request, ServerCallContext context)
        {
            var appInfoId = request.AppInfoId.Id;
            var appInfo = _dbContext.AppInfos.Include(x => x.AppInfoDetails)
                .Include(x => x.ChildAppInfos)
                .ThenInclude(x => x.AppInfoDetails)
                .SingleOrDefault(x => x.Id == appInfoId);
            if (appInfo == null)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "AppInfo not exists."));
            }
            var response = new GetBoundAppInfosResponse();
            if (appInfo.IsInternal)
            {
                response.AppInfos.Add(appInfo.ToProtoAppInfo());
                response.AppInfos.AddRange(appInfo.ChildAppInfos.Select(x => x.ToProtoAppInfo()));
            }
            else
            {
                if (appInfo.ParentAppInfo != null)
                {
                    response.AppInfos.Add(appInfo.ParentAppInfo.ToProtoAppInfo());
                }
                response.AppInfos.Add(appInfo.ToProtoAppInfo());
            }
            return Task.FromResult(response);
        }
    }
}
