using BackgroundWorker.Api.ClientMq;
using BackgroundWorker.Api.Kafka;
using BackgroundWorker.Api.Models;
using BackgroundWorker.Api.MqQueueReader;
using Confluent.Kafka;

namespace BackgroundWorker.Api.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddArchitectures(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();
        builder.Services.InsertDependencyInjection();
        builder.Services.AddIbmMQ();
        builder.Services.AddKafka();
        builder.Services.AddHostedService<BackgroundWorkerService>();
        return builder;
    }

    /// <summary>
    /// Insere e inicia o service do MQ - queueOptions
    /// </summary>
    /// <param name="services"></param>
    public static void AddIbmMQ(this IServiceCollection services)
    {
        QueueOptions queueOptions;
        //TODO: Refactor to avoid BuildServiceProvider
        using (var serviceProvider = services.BuildServiceProvider())
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            services.Configure<QueueOptions>(configuration.GetSection("queueOptions"));
            queueOptions = configuration.GetOptions<QueueOptions>("queueOptions");
        }
        services.AddSingleton(queueOptions);
    }
    
    public static void AddKafka(this IServiceCollection services)
    {
        //TODO: Refactor to avoid BuildServiceProvider
        using var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    
        var kafkaConfig = configuration.GetSection("KafkaConfig");
        var bootstrapServer = kafkaConfig.GetValue<string>("BootstrapServer");
        var groupId = kafkaConfig.GetValue<string>("GroupId") ?? "Group-1"; 
    
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrapServer,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
    
        services.AddSingleton<IConsumer<Ignore, string>>(_ =>
            new ConsumerBuilder<Ignore, string>(consumerConfig).Build());
    }

    private static void InsertDependencyInjection(this IServiceCollection services)
    {
        services.AddTransient<IMQClient, MQClient>();
        services.AddSingleton<IQueueReader, QueueReader>();
        services.AddTransient<IConsumerService, ConsumerService>();
        services.AddTransient<ProducerService>();
    }
}
