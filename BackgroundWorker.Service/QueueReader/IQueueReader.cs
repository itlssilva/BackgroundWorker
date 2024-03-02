namespace BackgroundWorker.Service.QueueReader;

public interface IQueueReader
{
    QueueMessage Dequque(string messageId);
    QueueMessage Peek();
}