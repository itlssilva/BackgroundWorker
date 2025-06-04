namespace BackgroundWorker.Api.Kafka;

public interface IConsumerService
{
    string GetMessageAsync(CancellationToken stoppingToken);
    void ConsumerServiceClose();
}
