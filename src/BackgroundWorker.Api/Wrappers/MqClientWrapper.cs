using BackgroundWorker.Api.ClientMq;
using BackgroundWorker.Api.Models;
using IBM.WMQ;

namespace BackgroundWorker.Api.Wrappers
{
    public class MqClientWrapper : IMqClientWrapper
    {
        private readonly IMQClient _mqClient;

        public MqClientWrapper(IMQClient mqClient)
        {
            _mqClient = mqClient;
        }

        public IMqQueue GetQueue(string queueName, int openOptions)
        {
            return new MqQueueWrapper(_mqClient.GetQueue(queueName, openOptions));
        }

        public IMqQueue GetResilientQueue(int openOptions, EParametersQueue parametersQueue)
        {
            return new MqQueueWrapper(_mqClient.GetResilientQueue(openOptions, parametersQueue));
        }
    }
}
