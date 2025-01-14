using System.Threading.Tasks;
using BackgroundWorker.Api.Kafka;
using BackgroundWorker.Api.MqQueueReader;

namespace BackgroundWorker.Api;

public class BackgroundWorkerService : BackgroundService
{
    // private Timer _timer;
    // private int cont;
    private readonly IQueueReader _queueReader;
    private readonly IConsumerService _consumerService;

    public BackgroundWorkerService(IQueueReader queueReader, IConsumerService consumerService)
    {
        _queueReader = queueReader;
        _consumerService = consumerService;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
         while (!stoppingToken.IsCancellationRequested)
        {
            await DoWork(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }  
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        // string machine = Dns.GetHostName();
        // IPAddress[] ip = Dns.GetHostAddresses(machine);
        // Console.WriteLine($"{ip[6]} => Contagem {cont}");
        // cont++;
        // var ret = _queueReader.Dequque("");
        // var ret = _queueReader.Peek();
        var ret = _consumerService.GetMessageAsync(stoppingToken);
        if (ret != null)
            Console.WriteLine(ret);
            // Console.WriteLine(ret.Data.Trim());
        //Console.WriteLine(ret?.Data);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumerService.ConsumerServiceClose();
        // return base.StopAsync(cancellationToken);
        return Task.CompletedTask;
    }
}