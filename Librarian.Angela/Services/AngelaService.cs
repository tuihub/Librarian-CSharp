using Librarian.Common;
using Microsoft.Extensions.Logging;

namespace Librarian.Angela.Services
{
    public partial class AngelaService
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