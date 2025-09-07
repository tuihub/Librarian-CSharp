using Librarian.Common;
using Microsoft.Extensions.Logging;

namespace Librarian.Angela.Services;

public partial class AngelaService : V1.AngelaService.AngelaServiceBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger _logger;

    public AngelaService(ILogger<AngelaService> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
}