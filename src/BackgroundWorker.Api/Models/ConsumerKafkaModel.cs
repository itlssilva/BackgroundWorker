namespace BackgroundWorker.Api.Models;

public class ConsumerKafkaModel
{
    public ConsumerKafkaModel(string bootstrapServer, string topic)
    {
        BootstrapServer = bootstrapServer;
        TopicName = topic;
    }

    public string BootstrapServer { get; set; }
    public string TopicName { get; set; }
    public string Group { get; set; } = "Group 1";
}
