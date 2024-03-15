using Grpc.Core;
using Librarian.Porter.Configs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Porter.Services
{
    public partial class PorterService : LibrarianPorterService.LibrarianPorterServiceBase
    {
        private readonly ILogger _logger;
        private readonly PorterConfig _porterConfig;
        private readonly AppInfoServiceResolver _appInfoServiceResolver;
        public PorterService(ILogger<PorterService> logger, PorterConfig porterConfig,
            AppInfoServiceResolver appInfoServiceResolver)
        {
            _logger = logger;
            _porterConfig = porterConfig;
            _appInfoServiceResolver = appInfoServiceResolver;
        }
    }
}
