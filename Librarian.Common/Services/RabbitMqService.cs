using Librarian.Common.Contracts;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Librarian.Common.Services
{
    // TODO: Implement caching for RabbitMQ connections
    public class RabbitMqService : IMessageQueueService
    {
        private readonly ILogger _logger;
        private readonly IConnection _connection;
        public RabbitMqService(ILogger<RabbitMqService> logger, IConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public void PublishMessage(string queueName, object message)
        {
            byte[] bodyBytes;
            if (message is string messageStr)
            {
                bodyBytes = Encoding.UTF8.GetBytes(messageStr);
            }
            else
            {
                bodyBytes = JsonSerializer.SerializeToUtf8Bytes(message);
            }
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: bodyBytes);
        }

        public void PublishMessage(object? channelObj, string queueName, object message)
        {
            var channel = (channelObj as IModel)!;
            byte[] bodyBytes;
            if (message is string messageStr)
            {
                bodyBytes = Encoding.UTF8.GetBytes(messageStr);
            }
            else
            {
                bodyBytes = JsonSerializer.SerializeToUtf8Bytes(message);
            }
            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: bodyBytes);
        }

        public void SubscribeQueue(object? channelObj, string queueName, Func<object, CancellationToken, Task> callback, Type objType, CancellationToken ct = default)
        {
            var channel = (channelObj as IModel)!;
            _logger.LogDebug($"Registering {queueName} for channel {channel}");
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (ch, ea) =>
            {
                _logger.LogDebug($"{ch} received {ea.Body}");
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var obj = JsonSerializer.Deserialize(message, objType);
                if (obj == null)
                {
                    _logger.LogWarning("Failed to deserialize message: {message}", message);
                    channel.BasicNack(ea.DeliveryTag, false, false);
                }
                else
                {
                    try { await callback(obj, ct); }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error processing message: {message}", message);
                        channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
            _logger.LogDebug($"Registeration for {queueName} complete");
        }

        public void SubscribeQueue(object? channelObj, string queueName, Action<object, CancellationToken> callback, Type objType, CancellationToken ct = default)
        {
            var channel = (channelObj as IModel)!;
            SubscribeQueue(channel, queueName, (appIdMQ, ct) =>
            {
                callback(appIdMQ, ct);
                return Task.CompletedTask;
            }, objType, ct);
        }

        public void UnsubscribeQueue(object? channelObj, string queueName)
        {
            try
            {
                var channel = (channelObj as IModel)!;
                channel.BasicCancel(queueName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to unsubscribe from {queueName}", queueName);
            }
        }
    }
}
