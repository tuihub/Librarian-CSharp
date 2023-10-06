using Grpc.Core;
using Librarian.Angela.Services;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PullMetadataService _pullMetadataService;
        public SephirahService(ApplicationDbContext dbContext, PullMetadataService pullMetadataService)
        {
            _dbContext = dbContext;
            _pullMetadataService = pullMetadataService;
        }
    }
}
