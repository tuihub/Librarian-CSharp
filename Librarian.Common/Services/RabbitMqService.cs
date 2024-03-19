using Librarian.Common.Contracts;
using Librarian.Common.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Librarian.Common.Services
{
    public class RabbitMqService : IMessageQueueService
    {
        private readonly ILogger _logger;
        private readonly IConnection _connection;
        public RabbitMqService(ILogger<RabbitMqService> logger, IConnection connection)
        {
            _logger = logger;
            _connection = connection;
        }

        public void PublishMessage(string queueName, string message)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: Encoding.UTF8.GetBytes(message));
        }
        public void SubscribeQueue(string queueName, Func<AppIdMQ, Task> callback, CancellationToken cts = default)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += async (ch, ea) =>
            {
                if (cts.IsCancellationRequested)
                {
                    _logger.LogInformation("Cancelling subscription");
                    return;
                }
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var appIdMQ = JsonSerializer.Deserialize<AppIdMQ>(message);
                if (appIdMQ == null)
                {
                    _logger.LogWarning("Failed to deserialize message: {message}", message);
                    channel.BasicNack(ea.DeliveryTag, false, false);
                }
                else
                {
                    try { await callback(appIdMQ); }
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
        }
        public void SubscribeQueue(string queueName, Action<AppIdMQ> callback, CancellationToken cts = default)
        {
            SubscribeQueue(queueName, (appIdMQ) =>
            {
                callback(appIdMQ);
                return Task.CompletedTask;
            }, cts);
        }
    }
}
