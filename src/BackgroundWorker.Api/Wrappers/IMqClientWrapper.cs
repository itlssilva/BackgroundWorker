using BackgroundWorker.Api.Models;
using IBM.WMQ;

namespace BackgroundWorker.Api.Wrappers
{
    public interface IMqClientWrapper
    {
        IMqQueue GetQueue(string queueName, int openOptions);
        IMqQueue GetResilientQueue(int openOptions, EParametersQueue parametersQueue);
    }
}
