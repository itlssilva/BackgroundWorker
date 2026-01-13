using BackgroundWorker.Api.Models;
using Confluent.Kafka;

namespace BackgroundWorker.Api.Kafka;

public class ConsumerService : IConsumerService
{
    private readonly IConfiguration _configuration;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<ConsumerService> _logger;
    private readonly ConsumerKafkaModel _consumerKafkaModel;

    public ConsumerService(IConfiguration configuration, ILogger<ConsumerService> logger, IConsumer<Ignore, string> consumer)
    {
        _configuration = configuration;
        _logger = logger;
        _consumer = consumer;

        var bootstrapServer = _configuration.GetSection("KafkaConfig").GetSection("BootstrapServer").Value;
        var topic = _configuration.GetSection("KafkaConfig").GetSection("TopicName").Value;
        _consumerKafkaModel = new ConsumerKafkaModel(bootstrapServer, topic);
    }

    public void ConsumerServiceClose()
    {
        _consumer.Close();
        _logger.LogInformation("Serviço Parado");
    }

    public string GetMessageAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Aguardando mensagens");
        _consumer.Subscribe(_consumerKafkaModel.TopicName);
        var result = _consumer.Consume(stoppingToken);
        return result.Message.Value;
    }
}
