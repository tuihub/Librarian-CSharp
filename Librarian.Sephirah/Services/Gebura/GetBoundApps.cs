using Grpc.Core;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
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
        /// <para>Get all associated Apps when requested AppSource is INTERNAL</para>
        /// <para>Get the exact App when requested AppSource IS NOT INTERNAL</para>
        /// </returns>
        [Authorize]
        public override Task<GetBoundAppsResponse> GetBoundApps(GetBoundAppsRequest request, ServerCallContext context)
        {
            var appId = request.AppId.Id;
            var app = _dbContext.Apps.SingleOrDefault(x => x.Id == appId);
            if (app == null) 
                throw new RpcException(new Status(StatusCode.InvalidArgument, "App not exists."));
            var ret = new GetBoundAppsResponse();
            if (app.IsInternal)
            {
                // TODO: App which AppSource IS NOT INTERNAL bind to App which AppSource is INTERNAL
                ret.Apps.Add(app.ToProtoApp());
            }
            else
            {
                ret.Apps.Add(app.ToProtoApp());
            }
            return Task.FromResult(ret);
        }
    }
}
