using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarianTests.IntegrationTests.Services.Sephirah
{
    public class SephirahBase
    {
        private GrpcChannel? _channel;

        protected GrpcChannel Channel => _channel ??= CreateChannel();

        protected GrpcChannel CreateChannel()
        {
            return GrpcChannel.ForAddress(GlobalContext.SephirahServiceAddr);
        }
    }
}
