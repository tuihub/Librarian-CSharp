using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.Collections.Concurrent;

namespace Librarian.Common.Services
{
    public class PorterGrpcClientPool<TClient> where TClient : class
    {
        private readonly ILogger<PorterGrpcClientPool<TClient>> _logger;
        private readonly ConcurrentDictionary<string, GrpcChannel> _channels = new();
        private readonly ConcurrentDictionary<string, TClient> _clients = new();
        private readonly Func<GrpcChannel, TClient> _clientFactory;
        private readonly AsyncRetryPolicy _retryPolicy;

        public PorterGrpcClientPool(
            ILogger<PorterGrpcClientPool<TClient>> logger,
            Func<GrpcChannel, TClient> clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;

            _retryPolicy = Policy
                .Handle<RpcException>(ex =>
                    ex.StatusCode == StatusCode.Unavailable ||
                    ex.StatusCode == StatusCode.ResourceExhausted)
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time, retryCount, context) =>
                    {
                        if (retryCount > 0)
                        {
                            _logger.LogWarning(ex, "Connect to Porter failed, retrying in {time}, retryCount = {retryCount}", time, retryCount);
                        }
                    });
        }

        /// <summary>
        /// Get or create a gRPC client for the specified URL
        /// </summary>
        public TClient GetClient(string url)
        {
            return _clients.GetOrAdd(url, CreateClient);
        }

        public Task<TResponse> ExecuteWithRetryAsync<TResponse>(
            string url,
            Func<TClient, CancellationToken, Task<TResponse>> action,
            CancellationToken ct = default)
        {
            var client = GetClient(url);

            return _retryPolicy.ExecuteAsync(async token =>
            {
                try
                {
                    return await action(client, token);
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
                {
                    _logger.LogWarning(ex, "Porter {Url} is unavailable, attempting to recreate client", url);

                    if (_clients.TryRemove(url, out _) && _channels.TryRemove(url, out var channel))
                    {
                        await channel.ShutdownAsync();
                    }

                    client = GetClient(url);

                    // throw to trigger retry
                    throw;
                }
            }, ct);
        }

        private TClient CreateClient(string url)
        {
            var channel = _channels.GetOrAdd(url, CreateChannel);
            return _clientFactory(channel);
        }

        private GrpcChannel CreateChannel(string url)
        {
            _logger.LogDebug("Creating gRPC channel to {Url}", url);

            var channel = GrpcChannel.ForAddress(url, new GrpcChannelOptions
            {
                MaxReceiveMessageSize = 16 * 1024 * 1024, // 16MB
                MaxSendMessageSize = 16 * 1024 * 1024 // 16MB
            });

            return channel;
        }

        public async Task ShutdownAllAsync()
        {
            foreach (var channel in _channels.Values)
            {
                await channel.ShutdownAsync();
            }

            _channels.Clear();
            _clients.Clear();
        }
    }
}