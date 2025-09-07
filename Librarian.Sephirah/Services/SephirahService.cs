using IdGen;
using Librarian.Angela.Services;
using Librarian.Common;
using Librarian.Common.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services;

public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
{
    private readonly IBus _bus;
    private readonly ApplicationDbContext _dbContext;
    private readonly IdGenerator _idGenerator;
    private readonly ILogger _logger;
    private readonly PullMetadataService _pullMetadataService;
    private readonly SephirahContext _sephirahContext;

    public SephirahService(ILogger<SephirahService> logger, ApplicationDbContext dbContext,
        PullMetadataService pullMetadataService,
        SephirahContext sephirahContext, IdGenerator idGenerator, IBus bus)
    {
        _logger = logger;
        _dbContext = dbContext;
        _pullMetadataService = pullMetadataService;
        _sephirahContext = sephirahContext;
        _idGenerator = idGenerator;
        _bus = bus;
    }

    // Example method for sending AppId messages
    protected async Task SendAppIdMessageAsync(string appId, string platform)
    {
        try
        {
            var message = new AppIdMQ
            {
                AppId = appId
            };

            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{platform}"));
            await endpoint.Send(message);

            _logger.LogDebug("Sent AppId message to platform {platform}: {message}", platform, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send AppId message to platform {platform}: {appId}", platform, appId);
        }
    }
}