using BackgroundWorker.Service;
using BackgroundWorker.Service.MQClient;
using BackgroundWorker.Service.QueueReader;
using Microsoft.AspNetCore.ResponseCompression;

namespace BackgroundWorker.Api.ApiConfiguration;

public static class ApiConfigurations
{
    /// <summary>
    /// Insere a compressão Brotli nas requisiçoes
    /// </summary>
    /// <param name="services"></param>
    public static void InsertRequisitionCompression(this IServiceCollection services)
    {
        services.AddResponseCompression(options =>
        {
            options.Providers.Add<BrotliCompressionProvider>();
            options.EnableForHttps = true;
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
        });
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    public static void InsertDependencyInjection(this IServiceCollection services)
    {
        services.AddSingleton<IMQClient, MQClient>();
        services.AddSingleton<IQueueReader, QueueReader>();
    }
}