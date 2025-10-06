using AutoMapper;
using Librarian.Angela.Mapping;
using Librarian.Common;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Angela.Services;

public partial class AngelaService : Librarian.Sephirah.Angela.AngelaService.AngelaServiceBase
{
    private static readonly IMapper s_mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<StoreAppMappingProfile>();
        cfg.AddProfile<TipherethMappingProfile>();
        cfg.AddProfile<SentinelMappingProfile>();
    }).CreateMapper();

    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly LibrarianSephirahService.LibrarianSephirahServiceClient _sephirahClient;

    public AngelaService(ILogger<AngelaService> logger, ApplicationDbContext dbContext,
        LibrarianSephirahService.LibrarianSephirahServiceClient sephirahClient)
    {
        _logger = logger;
        _dbContext = dbContext;
        _sephirahClient = sephirahClient;
    }
}