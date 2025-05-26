using Librarian.ThirdParty.Contracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Librarian.ThirdParty.Vndb
{
    public partial class VndbTcpApiService : IAppInfoService
    {
        private static readonly JsonSerializerOptions s_jso_urje = new() { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        // Client configuration parameters
        private readonly TimeSpan _timeout = new TimeSpan(0, 0, 30);
        private readonly string _clientName = "Librarian-CSharp";
        private readonly string _clientVersion = "0.2";
        private readonly ILogger<VndbTcpApiService> _logger;

        public VndbTcpApiService(ILogger<VndbTcpApiService> logger)
        {
            _logger = logger;
        }

        // Helper method to create and configure a VNDB client
        private VndbSharp.Vndb CreateVndbClient()
        {
            return new VndbSharp.Vndb(useTls: true)
                .WithTimeout(_timeout)
                .WithClientDetails(_clientName, _clientVersion)
                .WithFlagsCheck(true, (method, providedFlags, invalidFlags) =>
                {
                    throw new Exception($"Attempted to use method \"{method}\" with flags {providedFlags}," +
                        $" but {invalidFlags} are not permitted on that method.");
                });
        }
    }
}
