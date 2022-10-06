using Grpc.Core;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Services.Sephirah
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        public override Task<GetTokenResponse> GetToken(GetTokenRequest request, ServerCallContext context)
        {
            return base.GetToken(request, context);
        }
    }
}
