using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Sephirah.Contracts
{
    public interface IMessageQueueService
    {
        public void PublishMessage(string queueName, string message);
        public void SubscribeQueue(string queueName, Action<string> callback);
        public void SubscribeQueueAsync(string queueName, Func<string, Task> callback);
    }
}
