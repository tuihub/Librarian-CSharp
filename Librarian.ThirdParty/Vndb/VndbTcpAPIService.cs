using System.Text.Encodings.Web;
using System.Text.Json;
using Librarian.ThirdParty.Contracts;
using Microsoft.Extensions.Logging;

namespace Librarian.ThirdParty.Vndb;

public partial class VndbTcpApiService : IAppInfoService
{
    private static readonly JsonSerializerOptions s_jso_urje = new()
        { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    private readonly string _clientName = "Librarian-CSharp";
    private readonly string _clientVersion = "0.2";
    private readonly ILogger<VndbTcpApiService> _logger;

    // Client configuration parameters
    private readonly TimeSpan _timeout = new(0, 0, 30);

    public VndbTcpApiService(ILogger<VndbTcpApiService> logger)
    {
        _logger = logger;
    }

    // Helper method to create and configure a VNDB client
    private VndbSharp.Vndb CreateVndbClient()
    {
        return new VndbSharp.Vndb(true)
            .WithTimeout(_timeout)
            .WithClientDetails(_clientName, _clientVersion)
            .WithFlagsCheck(true, (method, providedFlags, invalidFlags) =>
            {
                throw new Exception($"Attempted to use method \"{method}\" with flags {providedFlags}," +
                                    $" but {invalidFlags} are not permitted on that method.");
            });
    }
}