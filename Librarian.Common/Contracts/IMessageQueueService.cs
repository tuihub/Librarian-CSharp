using Librarian.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Common.Contracts
{
    public interface IMessageQueueService
    {
        public void PublishMessage(string queueName, string message);
        public void PublishMessage(object? channel, string queueName, string message);
        public void SubscribeQueue(object? channel, string queueName, Action<AppIdMQ, CancellationToken> callback, CancellationToken cts = default);
        public void SubscribeQueue(object? channel, string queueName, Func<AppIdMQ, CancellationToken, Task> callback, CancellationToken cts = default);
        public void UnsubscribeQueue(object? channelObj, string queueName);
    }
}
