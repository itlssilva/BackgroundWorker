using BackgroundWorker.Api.MqQueueReader;

namespace BackgroundWorker.Api;

public class BackgroundWorkerService : BackgroundService
{
    // private Timer _timer;
    // private int cont;
    private readonly IQueueReader _queueReader;

    public BackgroundWorkerService(IQueueReader queueReader)
    {
        _queueReader = queueReader;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
         while (!stoppingToken.IsCancellationRequested)
        {
            DoWork();
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }  
    }

    private void DoWork(/*object state*/)
    {
        // string machine = Dns.GetHostName();
        // IPAddress[] ip = Dns.GetHostAddresses(machine);
        // Console.WriteLine($"{ip[6]} => Contagem {cont}");
        // cont++;
        var ret = _queueReader.Dequque("");
        // var ret = _queueReader.Peek();
        if (ret != null)
            Console.WriteLine(ret.Data.Trim());
        //Console.WriteLine(ret?.Data);
    }    
}