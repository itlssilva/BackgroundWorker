namespace BackgroundWorker.Service;

public class QueueMessage
{
    public string CorrelationId { get; set; }
    public string Data { get; set; }
    public QueueMessage() { }
    public QueueMessage(string data)
    {
        CorrelationId = $"1234{DateTime.Now:hhmmssff}";
        Data = data;
    }
}