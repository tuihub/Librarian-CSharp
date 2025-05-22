using Librarian.Common;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1.Angela;

namespace Librarian.Angela.Services
{
    public partial class AngelaService : LibrarianAngelaService.LibrarianAngelaServiceBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;

        public AngelaService(ILogger<AngelaService> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}