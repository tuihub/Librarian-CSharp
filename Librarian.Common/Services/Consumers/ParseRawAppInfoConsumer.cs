using System.Text.Json;
using Librarian.Common.Models.Db;
using Librarian.Common.Models.Mq;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TuiHub.Protos.Librarian.V1;
using FeatureRequest = TuiHub.Protos.Librarian.V1.FeatureRequest;

namespace Librarian.Common.Services.Consumers;

public class ParseRawAppInfoConsumer : PorterConsumerBase<ParseRawAppInfo>
{
    private readonly ILogger<ParseRawAppInfoConsumer> _logger;
    private readonly LibrarianPorterClientService _porterClientService;
    private readonly IServiceProvider _serviceProvider;

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
        ConsumeContext<ParseRawAppInfo> context,
        CancellationToken cancellationToken)
    {
        var request = context.Message;
        var region = request.Region;

        _logger.LogInformation("Processing ParseRawAppInfo request {RequestId} for source: {Source}, region: {Region}",
            request.RequestId, request.Source, region);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var featureRequest = new FeatureRequest
            {
                Id = request.Source,
                Region = region ?? string.Empty,
                ConfigJson = JsonSerializer.Serialize(new Models.FeatureRequests.ParseRawAppInfo())
            };
            var appInfo =
                await _porterClientService.ParseRawAppInfoAsync(featureRequest, request.RawDataJson, cancellationToken);

            if (appInfo != null)
            {
                // Find or create AppInfo record
                var existingAppInfo = await dbContext.AppInfos
                    .FirstOrDefaultAsync(
                        x => x.Source.ToString() == request.Source && x.SourceAppId == appInfo.SourceAppId,
                        cancellationToken);

                if (existingAppInfo != null)
                {
                    // Update existing record
                    UpdateAppInfoFromProto(existingAppInfo, appInfo);
                    _logger.LogInformation(
                        "Updated existing AppInfo record for source: {Source}, appId: {SourceAppId}, region: {Region}",
                        request.Source, appInfo.SourceAppId, region);
                }
                else
                {
                    // Create new record
                    var newAppInfo = CreateAppInfoFromProto(appInfo);
                    dbContext.AppInfos.Add(newAppInfo);
                    _logger.LogInformation(
                        "Created new AppInfo record for source: {Source}, appId: {SourceAppId}, region: {Region}",
                        request.Source, appInfo.SourceAppId, region);
                }

                await dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    "ParseRawAppInfo request {RequestId} completed successfully and persisted to database",
                    request.RequestId);
            }
            else
            {
                _logger.LogWarning("Received empty AppInfo response for source: {Source}, region: {Region}",
                    request.Source, region);
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

    private void UpdateAppInfoFromProto(AppInfo dbAppInfo, TuiHub.Protos.Librarian.Porter.V1.AppInfo protoAppInfo)
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

    private AppInfo CreateAppInfoFromProto(TuiHub.Protos.Librarian.Porter.V1.AppInfo protoAppInfo)
    {
        // TODO: Implement full mapping from proto AppInfo to database AppInfo
        return new AppInfo
        {
            Source = Enum.Parse<WellKnownAppInfoSource>(protoAppInfo.Source, true),
            SourceAppId = protoAppInfo.SourceAppId,
            SourceUrl = protoAppInfo.SourceUrl,
            Name = protoAppInfo.Name,
            Description = protoAppInfo.ShortDescription,
            IconImageUrl = protoAppInfo.IconImageUrl,
            BackgroundImageUrl = protoAppInfo.BackgroundImageUrl,
            CoverImageUrl = protoAppInfo.CoverImageUrl
            // Add other field mappings as needed
        };
    }
}