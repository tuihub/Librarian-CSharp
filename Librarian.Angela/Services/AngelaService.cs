using AutoMapper;
using IdGen;
using Librarian.Angela.Mapping;
using Librarian.Common;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Angela.Services;

public partial class AngelaService : Sephirah.Angela.AngelaService.AngelaServiceBase
{
    private static readonly IMapper s_mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile<StoreAppMappingProfile>();
        cfg.AddProfile<TipherethMappingProfile>();
        cfg.AddProfile<SentinelMappingProfile>();
    }).CreateMapper();

    private readonly ApplicationDbContext _dbContext;
    private readonly IdGenerator _idGenerator;
    private readonly ILogger _logger;

    public AngelaService(ILogger<AngelaService> logger, ApplicationDbContext dbContext, IdGenerator idGenerator)
    {
        _logger = logger;
        _dbContext = dbContext;
        _idGenerator = idGenerator;
    }
}