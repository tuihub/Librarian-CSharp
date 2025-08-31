using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TuiHub.Protos.Librarian.Porter.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Services.Consumers
{
    /// <summary>
    /// Consumer for handling app info search requests with database persistence
    /// Based on SearchAppInfoRequest from proto definition
    /// </summary>
    public class SearchAppInfoConsumer : PorterConsumerBase<Models.Mq.SearchAppInfo>
    {
        private readonly ILogger<SearchAppInfoConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly LibrarianPorterClientService _porterClientService;

        public SearchAppInfoConsumer(
            ILogger<SearchAppInfoConsumer> logger,
            LibrarianPorterClientService porterClientService,
            IServiceProvider serviceProvider)
            : base(logger, "SearchAppInfo")
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _porterClientService = porterClientService;
        }

        protected override async Task ProcessMessageAsync(
            ConsumeContext<Models.Mq.SearchAppInfo> context,
            CancellationToken cancellationToken)
        {
            var request = context.Message;
            var region = request.Region;

            _logger.LogInformation("Processing SearchAppInfo request {RequestId} for name like: {NameLike}, region: {Region}",
                request.RequestId, request.NameLike, region);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var featureRequest = new FeatureRequest
                {
                    Id = request.Source,
                    Region = region ?? string.Empty,
                    ConfigJson = JsonSerializer.Serialize(new Models.FeatureRequests.SearchAppInfo
                    {
                        NameLike = request.NameLike,
                    })
                };
                var appInfos = await _porterClientService.SearchAppInfoAsync(featureRequest, cancellationToken);

                if (appInfos != null && appInfos.Any())
                {
                    int createdCount = 0;
                    int updatedCount = 0;

                    foreach (var protoAppInfo in appInfos)
                    {
                        // Find existing AppInfo record
                        var existingAppInfo = await dbContext.AppInfos
                            .FirstOrDefaultAsync(x => x.Source.ToString() == protoAppInfo.Source && x.SourceAppId == protoAppInfo.SourceAppId,
                                cancellationToken);

                        if (existingAppInfo != null)
                        {
                            // Update existing record
                            UpdateAppInfoFromProto(existingAppInfo, protoAppInfo);
                            updatedCount++;
                        }
                        else
                        {
                            // Create new record
                            var newAppInfo = CreateAppInfoFromProto(protoAppInfo);
                            dbContext.AppInfos.Add(newAppInfo);
                            createdCount++;
                        }
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("SearchAppInfo request {RequestId} completed successfully. Created: {CreatedCount}, Updated: {UpdatedCount}, region: {Region}",
                        request.RequestId, createdCount, updatedCount, region);
                }
                else
                {
                    _logger.LogInformation("SearchAppInfo request {RequestId} returned no results for name like: {NameLike}, region: {Region}",
                        request.RequestId, request.NameLike, region);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("SearchAppInfo request {RequestId} was cancelled", request.RequestId);
                throw; // Re-throw to trigger MassTransit retry/dead letter handling
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing SearchAppInfo request {RequestId}", request.RequestId);
                throw; // Let MassTransit handle retry and dead letter queue
            }
        }

        private void UpdateAppInfoFromProto(Models.Db.AppInfo dbAppInfo, TuiHub.Protos.Librarian.Porter.V1.AppInfo protoAppInfo)
        {
            // TODO: Implement mapping from proto AppInfo to database AppInfo
            // This should be similar to the original PullAppInfoMetadataWorker logic
            dbAppInfo.Name = protoAppInfo.Name;
            dbAppInfo.Description = protoAppInfo.ShortDescription; // Map to Description field
            dbAppInfo.IconImageUrl = protoAppInfo.IconImageUrl;
            dbAppInfo.BackgroundImageUrl = protoAppInfo.BackgroundImageUrl;
            dbAppInfo.CoverImageUrl = protoAppInfo.CoverImageUrl;
            // Add other field mappings as needed
        }

        private Models.Db.AppInfo CreateAppInfoFromProto(TuiHub.Protos.Librarian.Porter.V1.AppInfo protoAppInfo)
        {
            // TODO: Implement full mapping from proto AppInfo to database AppInfo
            return new Models.Db.AppInfo
            {
                Source = Enum.Parse<WellKnownAppInfoSource>(protoAppInfo.Source, true),
                SourceAppId = protoAppInfo.SourceAppId,
                SourceUrl = protoAppInfo.SourceUrl,
                Name = protoAppInfo.Name,
                Description = protoAppInfo.ShortDescription,
                IconImageUrl = protoAppInfo.IconImageUrl,
                BackgroundImageUrl = protoAppInfo.BackgroundImageUrl,
                CoverImageUrl = protoAppInfo.CoverImageUrl,
                // Add other field mappings as needed
            };
        }
    }
}