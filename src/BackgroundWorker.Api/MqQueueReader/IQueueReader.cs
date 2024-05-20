using BackgroundWorker.Api.Models;

namespace BackgroundWorker.Api.MqQueueReader;

public interface IQueueReader
{
    QueueMessage Dequque(string messageId);
    QueueMessage Peek();
}