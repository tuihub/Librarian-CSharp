using Librarian.Common.Contracts;
using Librarian.Common.Models;
using Microsoft.Extensions.Logging;
using MemoryPack;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Librarian.Common.Services
{
    public class InMemoryMqService : IMessageQueueService
    {
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, Channel<byte[]>> _channels;

        public InMemoryMqService(ILogger<InMemoryMqService> logger)
        {
            _logger = logger;
            _channels = new ConcurrentDictionary<string, Channel<byte[]>>();
        }

        public void PublishMessage(string queueName, object message)
        {
            if (!_channels.TryGetValue(queueName, out var channel))
            {
                _logger.LogWarning("Queue {queueName} does not exist", queueName);
                return;
            }

            byte[] serializedMessage;
            if (message is string messageStr)
            {
                serializedMessage = System.Text.Encoding.UTF8.GetBytes(messageStr);
            }
            else
            {
                serializedMessage = MemoryPackSerializer.Serialize(message.GetType(), message);
            }

            if (!channel.Writer.TryWrite(serializedMessage))
            {
                _logger.LogError("Failed to write message to queue {queueName}", queueName);
            }
        }

        public void PublishMessage(object? channelObj, string queueName, object message)
        {
            PublishMessage(queueName, message);
        }

        public void SubscribeQueue(object? channelObj, string queueName, Func<object, CancellationToken, Task> callback, Type objType, CancellationToken ct = default)
        {
            var channel = _channels.GetOrAdd(queueName, _ => Channel.CreateBounded<byte[]>(new BoundedChannelOptions(GlobalContext.MessageQueueConfig.InMemoryMqConfig.MaxQueueSize)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            }));

            Task.Run(async () =>
            {
                await foreach (var message in channel.Reader.ReadAllAsync(ct))
                {
                    var obj = MemoryPackSerializer.Deserialize(objType, message);
                    if (obj == null)
                    {
                        _logger.LogWarning("Failed to deserialize message in queue {queueName}", queueName);
                        continue;
                    }

                    try
                    {
                        await callback(obj, ct);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message in queue {queueName}", queueName);
                    }
                }
            }, ct);
        }

        public void SubscribeQueue(object? channelObj, string queueName, Action<object, CancellationToken> callback, Type objType, CancellationToken ct = default)
        {
            SubscribeQueue(channelObj, queueName, (obj, ct) =>
            {
                callback(obj, ct);
                return Task.CompletedTask;
            }, objType, ct);
        }

        public void UnsubscribeQueue(object? channelObj, string queueName)
        {
            if (_channels.TryRemove(queueName, out var channel))
            {
                channel.Writer.Complete();
            }
            else
            {
                _logger.LogError("Failed to unsubscribe from {queueName}", queueName);
            }
        }
    }
}
