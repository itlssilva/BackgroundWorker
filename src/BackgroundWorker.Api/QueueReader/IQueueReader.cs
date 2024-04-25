using BackgroundWorker.Api.Models;

namespace BackgroundWorker.Api.QueueReader;

public interface IQueueReader
{
    QueueMessage Dequque(string messageId);
    QueueMessage Peek();
}