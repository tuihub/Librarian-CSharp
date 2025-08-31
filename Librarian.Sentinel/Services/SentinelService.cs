using Librarian.Common;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sentinel.V1;

namespace Librarian.Sentinel.Services
{
    public partial class SephirahSentinelService : LibrarianSephirahSentinelService.LibrarianSephirahSentinelServiceBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        public SephirahSentinelService(ILogger<SephirahSentinelService> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}
