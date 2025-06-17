using Grpc.Core;
using Librarian.Common.Models.Mq;
using MassTransit;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using TuiHub.Protos.Librarian.Porter.V1;

namespace Librarian.Common.Services.Consumers
{
    public abstract class PorterConsumerBase<TMessage>(
        ILogger logger,
        LibrarianPorterClientService porterClientService,
        string featureName) : IConsumer<TMessage>
        where TMessage : PorterMessageBase
    {
        private readonly ILogger _logger = logger;
        private readonly LibrarianPorterClientService _porterClientService = porterClientService;
        private readonly string _featureName = featureName;

        /// <summary>
        /// Gets the retry policy for this consumer. Can be overridden by derived classes.
        /// Default policy retries 3 times on Unavailable error.
        /// </summary>
        protected virtual AsyncRetryPolicy RetryPolicy => Policy
            .Handle<RpcException>(ex => ex.StatusCode == StatusCode.Unavailable)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time, retryCount, context) =>
                {
                    if (retryCount > 0)
                    {
                        _logger.LogWarning(ex, "Porter operation failed, retrying in {Time} seconds, attempt {RetryCount}",
                            time.TotalSeconds, retryCount);
                    }
                });

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var message = context.Message;
            var region = message.Region;

            _logger.LogDebug("Starting message processing, feature: {Feature}, region: {Region}", _featureName, region);

            try
            {
                // Use LibrarianPorterClientService which handles Porter acquisition, retry logic, and resource management
                await ProcessMessageAsync(context, context.CancellationToken);

                _logger.LogDebug("Successfully processed message, feature: {Feature}, region: {Region}", _featureName, region);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Task was cancelled, feature: {Feature}, region: {Region}", _featureName, region);
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.ResourceExhausted)
            {
                _logger.LogError(ex, "Resource exhausted while processing message, feature: {Feature}, region: {Region}", _featureName, region);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consumer error occurred while processing message, feature: {Feature}, region: {Region}", _featureName, region);
                throw;
            }
        }

        protected abstract Task ProcessMessageAsync(
            ConsumeContext<TMessage> context,
            CancellationToken cancellationToken);

        protected async Task<TResult?> ExecuteWithPorterAsync<TResult>(
            string? region,
            Func<LibrarianPorterService.LibrarianPorterServiceClient, CancellationToken, Task<TResult?>> action,
            CancellationToken cancellationToken)
        {
            return await RetryPolicy.ExecuteAsync(async (token) =>
            {
                return await _porterClientService.ExecuteWithPorterAsync(_featureName, region, action, token);
            }, cancellationToken);
        }

        protected Task<TResult?> ExecuteWithPorterDirectAsync<TResult>(
            string? region,
            Func<LibrarianPorterService.LibrarianPorterServiceClient, CancellationToken, Task<TResult?>> action,
            CancellationToken cancellationToken)
        {
            return _porterClientService.ExecuteWithPorterAsync(_featureName, region, action, cancellationToken);
        }
    }
}