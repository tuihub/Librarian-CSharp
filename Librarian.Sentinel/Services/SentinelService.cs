using Librarian.Common;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1.Sentinel;

namespace Librarian.Sentinel.Services
{
    public partial class SentinelService : LibrarianSentinelService.LibrarianSentinelServiceBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        public SentinelService(ILogger<SentinelService> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}
