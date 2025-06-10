using Librarian.Porter.Helpers;
using Librarian.Porter.Models;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService : LibrarianPorterService.LibrarianPorterServiceBase
    {
        private readonly ILogger _logger;
        private readonly GlobalContext _globalContext;
        private readonly AppInfoServiceResolver _appInfoServiceResolver;
        private readonly AccountServiceResolver _accountServiceResolver;

        public PorterService(ILogger<PorterService> logger, GlobalContext globalContext,
            AppInfoServiceResolver appInfoServiceResolver, AccountServiceResolver accountServiceResolver)
        {
            _logger = logger;
            _globalContext = globalContext;
            _appInfoServiceResolver = appInfoServiceResolver;
            _accountServiceResolver = accountServiceResolver;
        }
    }
}
