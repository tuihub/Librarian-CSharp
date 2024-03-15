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
        public override Task<PullAppInfoResponse> PullAppInfo(PullAppInfoRequest request, ServerCallContext context)
        {
            return base.PullAppInfo(request, context);
        }
    }
}
