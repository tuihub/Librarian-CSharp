namespace Librarian.Common.Contracts
{
    public interface IMessageQueueService
    {
        public void PublishMessage(string queueName, object message);
        public void PublishMessage(object? channelObj, string queueName, object message);
        public void SubscribeQueue(object? channelObj, string queueName, Action<object, CancellationToken> callback, Type objType, CancellationToken cts = default);
        public void SubscribeQueue(object? channelObj, string queueName, Func<object, CancellationToken, Task> callback, Type objType, CancellationToken cts = default);
        public void UnsubscribeQueue(object? channelObj, string queueName);
    }
}
