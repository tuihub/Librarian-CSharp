using IdGen;
using Librarian.Angela.Services;
using Librarian.Common;
using Librarian.Common.Contracts;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly PullMetadataService _pullMetadataService;
        private readonly SephirahContext _sephirahContext;
        private readonly IdGenerator _idGenerator;
        private readonly IMessageQueueService _messageQueueService;
        public SephirahService(ILogger<SephirahService> logger, ApplicationDbContext dbContext, PullMetadataService pullMetadataService,
            SephirahContext sephirahContext, IdGenerator idGenerator, IMessageQueueService messageQueueService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _pullMetadataService = pullMetadataService;
            _sephirahContext = sephirahContext;
            _idGenerator = idGenerator;
            _messageQueueService = messageQueueService;
        }
    }
}
