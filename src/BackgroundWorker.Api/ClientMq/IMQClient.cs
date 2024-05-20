using BackgroundWorker.Api.Models;
using IBM.WMQ;

namespace BackgroundWorker.Api.ClientMq;

public interface IMQClient
{
    MQQueue GetQueue(string queueName, int openOptions);
    MQQueue GetResilientQueue(int openOptions, EParametersQueue parametersQueue);
}