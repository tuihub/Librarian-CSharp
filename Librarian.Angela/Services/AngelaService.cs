using Librarian.Common;
using Microsoft.Extensions.Logging;
using Librarian.Angela.V1;

namespace Librarian.Angela.Services
{
    public partial class AngelaService : V1.AngelaService.AngelaServiceBase
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