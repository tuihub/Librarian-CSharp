using Grpc.Core;
using Librarian.Angela.Services;
using Librarian.Common.Utils;
using Microsoft.AspNetCore.Authorization;
using Minio;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly PullMetadataService _pullMetadataService;
        private readonly IMinioClient _minioClient;
        public SephirahService(ApplicationDbContext dbContext, PullMetadataService pullMetadataService, IMinioClient minioClient)
        {
            _dbContext = dbContext;
            _pullMetadataService = pullMetadataService;
            _minioClient = minioClient;
        }
    }
}
