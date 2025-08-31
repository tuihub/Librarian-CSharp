using Grpc.Core;
using Librarian.Common.Helpers;
using Librarian.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Sentinel.V1;
using DbModels = Librarian.Common.Models.Db;

namespace Librarian.Sentinel.Services
{
    public partial class SephirahSentinelService
    {
        // TODO: impl heartbeat check, commit snapshot
        public override async Task<ReportAppBinariesResponse> ReportAppBinaries(ReportAppBinariesRequest request, ServerCallContext context)
        {
            var sentinelId = context.GetInternalIdFromHeader();
            _logger.LogInformation("Received app binaries report from Sentinel ID: {SentinelId}", sentinelId);

            // Get sentinel with libraries
            var sentinel = await _dbContext.Sentinels
                .Include(s => s.SentinelLibraries)
                .ThenInclude(sl => sl.AppBinaries)
                .SingleOrDefaultAsync(s => s.Id == sentinelId, context.CancellationToken);

            if (sentinel == null)
            {
                _logger.LogWarning("Sentinel with ID {SentinelId} not found", sentinelId);
                throw new RpcException(new Status(StatusCode.NotFound, $"Sentinel with ID {sentinelId} not found"));
            }

            // Process app binaries
            var response = new ReportAppBinariesResponse();
            bool commitSnapshot = request.HasCommitSnapshot && request.CommitSnapshot;
            var snapshotTime = request.SnapshotTime?.ToDateTime() ?? DateTime.UtcNow;

            foreach (var library in sentinel.SentinelLibraries)
            {
                // get binaries in request
                var reportedBinaries = request.AppBinaries
                    .Where(b => b.SentinelLibraryId == library.LibraryId)
                    .ToList();
                if (reportedBinaries.Count == 0) { continue; }

                var existingAppBinaries = library.AppBinaries;

                var (toRemove, toAdd, toUpdate) = CollectionHelper.CompareCollections<DbModels.SentinelAppBinary, SentinelLibraryAppBinary, string>(
                    existingAppBinaries,
                    reportedBinaries,
                    oldItem => oldItem.GeneratedId,
                    newItem => newItem.SentinelGeneratedId,
                    newItem => new DbModels.SentinelAppBinary(library.Id, newItem),
                    (oldItem, convertedNewItem) => oldItem.Equals(convertedNewItem)
                );

                foreach (var binaryToRemove in toRemove)
                {
                    library.AppBinaries.Remove(binaryToRemove);
                    _logger.LogDebug("Removing app binary {BinaryId} from library {LibraryId}",
                        binaryToRemove.GeneratedId, library.Id);
                }
                foreach (var binaryToAdd in toAdd)
                {
                    library.AppBinaries.Add(binaryToAdd);
                    _logger.LogDebug("Adding new app binary {BinaryId} to library {LibraryId}",
                        binaryToAdd.GeneratedId, library.Id);
                }
                foreach (var (oldBinary, newBinary) in toUpdate)
                {
                    oldBinary.Update(newBinary);
                    _logger.LogDebug("Updating app binary {BinaryId} in library {LibraryId}",
                        oldBinary.GeneratedId, library.Id);
                }
            }

            await _dbContext.SaveChangesAsync(context.CancellationToken);

            // Handle snapshot commit
            if (commitSnapshot)
            {
                response.CommitSnapshotSuccess = true;
            }

            return response;
        }
    }
}