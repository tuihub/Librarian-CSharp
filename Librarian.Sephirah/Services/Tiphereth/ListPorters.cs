using Grpc.Core;
using Grpc.Net.Client;
using Librarian.Common.Utils;
using System.Text.Json;
using TuiHub.Protos.Librarian.Porter.V1;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        // TODO: impl enable porter
        public override Task<ListPortersResponse> ListPorters(ListPortersRequest request, ServerCallContext context)
        {
            var response = new ListPortersResponse();
            foreach (var service in _sephirahContext.PorterServices)
            {
                var channel = GrpcChannel.ForAddress($"http://{service.Address}:{service.Port}");
                var client = new LibrarianPorterService.LibrarianPorterServiceClient(channel);
                var porterInfo = client.GetPorterInformation(new GetPorterInformationRequest());
                response.Porters.Add(new Porter
                {
                    Id = new InternalID { Id = -1 },
                    Name = porterInfo.Name,
                    Version = porterInfo.Version,
                    GlobalName = porterInfo.GlobalName,
                    FeatureSummary = JsonSerializer.Serialize(service.Tags),
                    Status = UserStatus.Active,
                    ConnectionStatus = PorterConnectionStatus.Active
                });
                _pullMetadataService.EnablePorter(service.ID);
            }
            response.Paging = new PagingResponse { TotalSize = response.Porters.Count };
            return Task.FromResult(response);
        }
    }
}
