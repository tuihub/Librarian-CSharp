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
        public void SubscribeQueue(string queueName, Action<AppIdMQ> callback, CancellationToken cts = default);
        public void SubscribeQueue(string queueName, Func<AppIdMQ, Task> callback, CancellationToken cts = default);
    }
}
