using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Porter.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Services.Consumers
{
    public class ParseRawAppInfoConsumer : PorterConsumerBase<Models.Mq.ParseRawAppInfo>
    {
        private readonly ILogger<ParseRawAppInfoConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly LibrarianPorterClientService _porterClientService;

        public ParseRawAppInfoConsumer(
            ILogger<ParseRawAppInfoConsumer> logger,
            LibrarianPorterClientService porterClientService,
            IServiceProvider serviceProvider)
            : base(logger, "ParseRawAppInfo")
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _porterClientService = porterClientService;
        }

        protected override async Task ProcessMessageAsync(
            ConsumeContext<Models.Mq.ParseRawAppInfo> context,
            CancellationToken cancellationToken)
        {
            var request = context.Message;
            var region = request.Region;

            _logger.LogInformation("Processing ParseRawAppInfo request {RequestId} for source: {Source}, appId: {SourceAppId}, region: {Region}",
                request.RequestId, request.Source, request.SourceAppId, region);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var appInfo = await _porterClientService.ParseRawAppInfoAsync(request.Source, request.SourceAppId, request.RawDataJson, region, cancellationToken);

                if (appInfo != null)
                {
                    // Find or create AppInfo record
                    var existingAppInfo = await dbContext.AppInfos
                        .FirstOrDefaultAsync(x => x.Source.ToString() == request.Source && x.SourceAppId == request.SourceAppId,
                            cancellationToken);

                    if (existingAppInfo != null)
                    {
                        // Update existing record
                        UpdateAppInfoFromProto(existingAppInfo, appInfo);
                        _logger.LogInformation("Updated existing AppInfo record for source: {Source}, appId: {SourceAppId}, region: {Region}",
                            request.Source, request.SourceAppId, region);
                    }
                    else
                    {
                        // Create new record
                        var newAppInfo = CreateAppInfoFromProto(appInfo);
                        dbContext.AppInfos.Add(newAppInfo);
                        _logger.LogInformation("Created new AppInfo record for source: {Source}, appId: {SourceAppId}, region: {Region}",
                            request.Source, request.SourceAppId, region);
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("ParseRawAppInfo request {RequestId} completed successfully and persisted to database",
                        request.RequestId);
                }
                else
                {
                    _logger.LogWarning("Received empty AppInfo response for source: {Source}, appId: {SourceAppId}, region: {Region}",
                        request.Source, request.SourceAppId, region);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("ParseRawAppInfo request {RequestId} was cancelled", request.RequestId);
                throw; // Re-throw to trigger MassTransit retry/dead letter handling
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ParseRawAppInfo request {RequestId}", request.RequestId);
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