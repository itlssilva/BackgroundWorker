using BackgroundWorker.Api.Models;
using Confluent.Kafka;

namespace BackgroundWorker.Api.Kafka;

public class ConsumerService : IConsumerService
{
    private readonly IConfiguration _configuration;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ConsumerConfig _consumerConfig;
    private readonly ILogger<ConsumerService> _logger;
    private ConsumerKafkaModel consumerKafkaModel;

    public ConsumerService(IConfiguration configuration, ILogger<ConsumerService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        string bootstrapServer = _configuration.GetSection("KafkaConfig").GetSection("BootstrapServer").Value;
        string topic = _configuration.GetSection("KafkaConfig").GetSection("TopicName").Value;
        consumerKafkaModel = new(bootstrapServer, topic);
        _consumerConfig = new ConsumerConfig()
        {
            BootstrapServers = consumerKafkaModel.BootstrapServer,
            GroupId = consumerKafkaModel.Group,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
    }

    public void ConsumerServiceClose()
    {
        _consumer.Close();
        _logger.LogInformation("Serviço Parado");
    }

    public string GetMessageAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Aguardando mensagens");
        _consumer.Subscribe(consumerKafkaModel.TopicName);
        var result = _consumer.Consume(stoppingToken);
        return result.Message.Value;
    }
}
