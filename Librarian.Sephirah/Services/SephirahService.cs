using Grpc.Core;
using Librarian.Angela.Services;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Minio;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly PullMetadataService _pullMetadataService;
        private readonly IMinioClient _minioClient;
        public SephirahService(ILogger<SephirahService> logger ,ApplicationDbContext dbContext, PullMetadataService pullMetadataService, IMinioClient minioClient)
        {
            _logger = logger;
            _dbContext = dbContext;
            _pullMetadataService = pullMetadataService;
            _minioClient = minioClient;
        }
    }
}
