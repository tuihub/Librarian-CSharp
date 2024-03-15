using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService : LibrarianPorterService.LibrarianPorterServiceBase
    {
        public override Task<EnablePorterResponse> EnablePorter(EnablePorterRequest request, ServerCallContext context)
        {
            var sephirahId = request.SephirahId;
            if (StaticContext.SephirahId != null && StaticContext.SephirahId != sephirahId)
            {
                throw new RpcException(new Status(StatusCode.PermissionDenied, "Wrong sephirah id."));
            }
            StaticContext.SephirahId = sephirahId;
            return Task.FromResult(new EnablePorterResponse());
        }
    }
}
