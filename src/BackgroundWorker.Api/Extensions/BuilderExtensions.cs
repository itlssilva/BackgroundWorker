using BackgroundWorker.Api.ClientMq;
using BackgroundWorker.Api.Kafka;
using BackgroundWorker.Api.Models;
using BackgroundWorker.Api.MqQueueReader;

namespace BackgroundWorker.Api.Extensions;

public static class BuilderExtensions
{
    public static WebApplicationBuilder AddArchitectures(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHealthChecks();
        builder.Services.InsertDependencyInjection();
        builder.Services.AddIbmMQ();
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
        using (var serviceProvider = services.BuildServiceProvider())
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            services.Configure<QueueOptions>(configuration.GetSection("queueOptions"));
            queueOptions = configuration.GetOptions<QueueOptions>("queueOptions");
        }
        services.AddSingleton(queueOptions);
    }

    public static void InsertDependencyInjection(this IServiceCollection services)
    {
        services.AddTransient<IMQClient, MQClient>();
        services.AddSingleton<IQueueReader, QueueReader>();
        services.AddTransient<ProducerService>();
    }
}
