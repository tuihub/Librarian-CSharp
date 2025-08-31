using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TuiHub.Protos.Librarian.Sephirah.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Services
{
    public partial class SephirahService
    {
        /// <summary>
        /// Get store app details including binaries, save files and users
        /// </summary>
        [Authorize]
        public override async Task<GetStoreAppSummaryResponse> GetStoreAppSummary(GetStoreAppSummaryRequest request, ServerCallContext context)
        {
            // Get store app by id
            var storeApp = await _dbContext.StoreApps
                .FirstOrDefaultAsync(x => x.Id == request.StoreAppId.Id);

            if (storeApp == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Store app not found"));
            }

            // Create store app object
            var storeAppPb = new StoreApp
            {
                Id = new InternalID { Id = storeApp.Id },
                Name = storeApp.Name,
                Type = Common.Converters.EnumConverter.ToEnumByString<AppType>(storeApp.Type),
                Description = storeApp.Description,
                IconImageId = new InternalID { Id = storeApp.IconImageId },
                BackgroundImageId = new InternalID { Id = storeApp.BackgroundImageId },
                CoverImageId = new InternalID { Id = storeApp.CoverImageId },
                Developer = storeApp.Developer,
                Publisher = storeApp.Publisher,
                Public = storeApp.IsPublic
            };

            // Add tags and name alternatives
            storeAppPb.Tags.AddRange(storeApp.Tags);
            storeAppPb.NameAlternatives.AddRange(storeApp.AltNames);

            // Add app sources
            foreach (var source in storeApp.AppSources)
            {
                storeAppPb.BoundAppSource.Add(source.Key.ToString(), source.Value);
            }

            // Create response with basic store app info
            var response = new GetStoreAppSummaryResponse
            {
                StoreApp = new StoreAppSummary
                {
                    StoreApp = storeAppPb
                }
            };

            // Get app binaries if requested
            if (request.AppBinaryLimit > 0)
            {
                var binaries = await _dbContext.StoreAppBinaries
                    .Where(x => x.StoreAppId == storeApp.Id)
                    .OrderByDescending(x => x.UpdatedAt)
                    .Take((int)request.AppBinaryLimit)
                    .ToListAsync();

                foreach (var binary in binaries)
                {
                    var binaryPb = new StoreAppBinary
                    {
                        Id = new InternalID { Id = binary.Id },
                        Name = binary.Name,
                        // StoreAppBinary model does not have these fields in DB, using placeholders
                        SizeBytes = 0, // Would need to calculate from binary files
                        Public = false // Add this field to DB model if needed
                    };

                    response.StoreApp.Binaries.Add(binaryPb);
                }

                response.StoreApp.AppBinaryCount = await _dbContext.StoreAppBinaries
                    .Where(x => x.StoreAppId == storeApp.Id)
                    .CountAsync();
            }

            // Get app save files if requested
            if (request.AppSaveFileLimit > 0)
            {
                var saveFiles = await _dbContext.AppSaveFiles
                    .Include(x => x.FileMetadata)
                    .Include(x => x.App)
                    .Where(x => x.App != null && x.App.BoundStoreAppId == storeApp.Id)
                    .OrderByDescending(x => x.UpdatedAt)
                    .Take((int)request.AppSaveFileLimit)
                    .ToListAsync();

                foreach (var saveFile in saveFiles)
                {
                    var saveFilePb = new StoreAppSaveFile
                    {
                        Id = new InternalID { Id = saveFile.Id },
                        // AppSaveFile doesn't have these fields directly, get them from FileMetadata or set defaults
                        Name = saveFile.FileMetadata?.Name ?? "Unnamed",
                        Description = "", // Add to DB model if needed
                        Public = false // Add to DB model if needed
                    };

                    if (saveFile.FileMetadata != null)
                    {
                        saveFilePb.File = new FileMetadata
                        {
                            Id = new InternalID { Id = saveFile.FileMetadata.Id },
                            Name = saveFile.FileMetadata.Name,
                            SizeBytes = saveFile.FileMetadata.SizeBytes,
                            Type = (FileType)saveFile.FileMetadata.Type,
                            Sha256 = Google.Protobuf.ByteString.CopyFrom(saveFile.FileMetadata.Sha256),
                            CreateTime = Timestamp.FromDateTime(saveFile.FileMetadata.CreatedAt.ToUniversalTime())
                        };
                    }

                    response.StoreApp.SaveFiles.Add(saveFilePb);
                }

                response.StoreApp.AppSaveFileCount = await _dbContext.AppSaveFiles
                    .Where(x => x.App != null && x.App.BoundStoreAppId == storeApp.Id)
                    .CountAsync();
            }

            // Get acquired users if requested
            if (request.AcquiredUserLimit > 0)
            {
                var acquiredApps = await _dbContext.Apps
                    .Where(x => x.BoundStoreAppId == storeApp.Id)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take((int)request.AcquiredUserLimit)
                    .ToListAsync();

                foreach (var app in acquiredApps)
                {
                    // UserId is non-nullable in the App model
                    response.StoreApp.AcquiredUserIds.Add(new InternalID { Id = app.UserId });
                }

                response.StoreApp.AcquiredCount = await _dbContext.Apps
                    .Where(x => x.BoundStoreAppId == storeApp.Id)
                    .CountAsync();
            }

            return response;
        }
    }
}