using Grpc.Core;
using IdGen;
using Librarian.Angela.Services;
using Librarian.Common.Contracts;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Minio;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly PullMetadataService _pullMetadataService;
        private readonly IdGenerator _idGenerator;
        private readonly IMessageQueueService _messageQueueService;
        public SephirahService(ILogger<SephirahService> logger, ApplicationDbContext dbContext,
            PullMetadataService pullMetadataService, IdGenerator idGenerator, IMessageQueueService messageQueueService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _pullMetadataService = pullMetadataService;
            _idGenerator = idGenerator;
            _messageQueueService = messageQueueService;
        }
    }
}
