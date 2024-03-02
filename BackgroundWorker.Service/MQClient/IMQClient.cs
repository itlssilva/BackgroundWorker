using IBM.WMQ;

namespace BackgroundWorker.Service.MQClient;

public interface IMQClient
{
    MQQueue GetQueue(string queueName, int openOptions);
    MQQueue GetResilientQueue(int openOptions, EParametersQueue parametersQueue);
}