using Grpc.Core;
using Librarian.Sephirah.Utils;
using Microsoft.AspNetCore.Authorization;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService : LibrarianSephirahService.LibrarianSephirahServiceBase
    {
        private readonly ApplicationDbContext _dbContext;
        public SephirahService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
