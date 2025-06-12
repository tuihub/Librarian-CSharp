using MassTransit;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Librarian.Common.Services.Consumers
{
    public abstract class PorterConsumerBase<TMessage>(
        ILogger logger,
        PorterManagementService porterManagementService,
        string region) : IConsumer<TMessage>
        where TMessage : class
    {
        private readonly ILogger _logger = logger;
        private readonly PorterManagementService _porterManagementService = porterManagementService;
        private readonly string _region = region;

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var message = context.Message;
            var featureName = GetFeatureName();

            _logger.LogDebug("Starting message processing, feature: {Feature}, region: {Region}", featureName, _region);

            PorterInstance? porterInstance = null;
            var stopwatch = Stopwatch.StartNew();
            Exception? processingException = null;

            try
            {
                // Acquire Porter instance
                porterInstance = await _porterManagementService.AcquirePorterInstanceAsync(featureName, _region);

                if (porterInstance == null)
                {
                    throw new InvalidOperationException($"Unable to acquire available Porter instance for feature {featureName} in region {_region}");
                }

                _logger.LogDebug("Acquired Porter: {PorterId} ({Url}) for feature: {Feature}",
                    porterInstance.Id, porterInstance.Url, featureName);

                try
                {
                    // Process message
                    await ProcessMessageAsync(context, porterInstance, context.CancellationToken);

                    _logger.LogDebug("Successfully processed message, Porter: {PorterId}, feature: {Feature}, elapsed: {ElapsedMs}ms",
                        porterInstance.Id, featureName, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    processingException = ex;
                    _logger.LogError(ex, "Error occurred while processing message, Porter: {PorterId}, feature: {Feature}",
                        porterInstance.Id, featureName);
                    throw;
                }
                finally
                {
                    stopwatch.Stop();
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Task was cancelled, feature: {Feature}", featureName);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consumer error occurred while processing message, feature: {Feature}", featureName);
                throw;
            }
            finally
            {
                // Release Porter instance
                if (porterInstance != null)
                {
                    _porterManagementService.ReleasePorterInstance(porterInstance.Id, featureName);
                    _logger.LogDebug("Released Porter instance: {PorterId}, feature: {Feature}", porterInstance.Id, featureName);
                }
            }
        }

        /// <summary>
        /// Gets the Porter feature name corresponding to the current consumer
        /// </summary>
        protected abstract string GetFeatureName();

        /// <summary>
        /// Abstract method for actual message processing, implemented by derived classes
        /// </summary>
        protected abstract Task ProcessMessageAsync(
            ConsumeContext<TMessage> context,
            PorterInstance porter,
            CancellationToken cancellationToken);
    }
}