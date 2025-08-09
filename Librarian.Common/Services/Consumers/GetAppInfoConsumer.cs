using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.Porter.V1;
using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Services.Consumers
{
    public class GetAppInfoConsumer : PorterConsumerBase<Models.Mq.GetAppInfo>
    {
        private readonly ILogger<GetAppInfoConsumer> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly LibrarianPorterClientService _porterClientService;

        public GetAppInfoConsumer(
            ILogger<GetAppInfoConsumer> logger,
            LibrarianPorterClientService porterClientService,
            IServiceProvider serviceProvider)
            : base(logger, "GetAppInfo")
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _porterClientService = porterClientService;
        }

        protected override async Task ProcessMessageAsync(
            ConsumeContext<Models.Mq.GetAppInfo> context,
            CancellationToken cancellationToken)
        {
            var message = context.Message;
            var region = message.Region;

            _logger.LogInformation("Processing GetAppInfo request {RequestId} for source: {Source}, appId: {SourceAppId}, region: {Region}",
                message.RequestId, message.Source, message.SourceAppId, region);

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var appInfo = await _porterClientService.GetAppInfoAsync(message.Source, message.SourceAppId, region, cancellationToken);

                if (appInfo != null)
                {
                    // Find or create AppInfo record
                    var existingAppInfo = await dbContext.AppInfos
                        .FirstOrDefaultAsync(x => x.Source.ToString() == message.Source && x.SourceAppId == message.SourceAppId,
                            cancellationToken);

                    if (existingAppInfo != null)
                    {
                        // Update existing record
                        UpdateAppInfoFromProto(existingAppInfo, appInfo);
                        _logger.LogInformation("Updated existing AppInfo record for source: {Source}, appId: {SourceAppId}, region: {Region}",
                            message.Source, message.SourceAppId, region);
                    }
                    else
                    {
                        // Create new record
                        var newAppInfo = CreateAppInfoFromProto(appInfo);
                        dbContext.AppInfos.Add(newAppInfo);
                        _logger.LogInformation("Created new AppInfo record for source: {Source}, appId: {SourceAppId}, region: {Region}",
                            message.Source, message.SourceAppId, region);
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("GetAppInfo request {RequestId} completed successfully and persisted to database",
                        message.RequestId);
                }
                else
                {
                    _logger.LogWarning("Received empty AppInfo response for source: {Source}, appId: {SourceAppId}, region: {Region}",
                        message.Source, message.SourceAppId, region);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("GetAppInfo request {RequestId} was cancelled", message.RequestId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing GetAppInfo request {RequestId}", message.RequestId);
                throw;
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