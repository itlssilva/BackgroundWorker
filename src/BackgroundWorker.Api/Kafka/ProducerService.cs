using Confluent.Kafka;

namespace BackgroundWorker.Api.Kafka;

public class ProducerService
{
    private readonly IConfiguration _configuration;
    private readonly ProducerConfig _producerConfig;
    private readonly ILogger<ProducerService> _logger;

    public ProducerService(IConfiguration configuration, ILogger<ProducerService> logger)
    {
        _configuration = configuration;
        string bootstrapServer = _configuration.GetSection("KafkaConfig").GetSection("BootstrapServer").Value;
        _logger = logger;

        _producerConfig = new ProducerConfig()
        {
            BootstrapServers = bootstrapServer
        };
    }

    public async Task<string> SendMessage(string message)
    {
        try
        {
            string topic = _configuration.GetSection("KafkaConfig").GetSection("TopicName").Value;

            using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();
            var result = await producer.ProduceAsync(topic: topic, new() { Value = message });
            _logger.LogInformation($"{result.Status} - {message}");
            return $"{result.Status} - {message}";            
        }
        catch (Exception err)
        {
            _logger.LogError(err.Message);
            return $"Erro ao enviar mensagem. Erro => {err.Message}";
        }
    }
}