using IBM.WMQ;

namespace BackgroundWorker.Api.Wrappers
{
    public class MqQueueWrapper : IMqQueue
    {
        private readonly MQQueue _queue;

        public MqQueueWrapper(MQQueue queue)
        {
            _queue = queue;
        }

        public void Get(MQMessage message, MQGetMessageOptions options)
        {
            _queue.Get(message, options);
        }

        public void Close()
        {
            _queue.Close();
        }

        public void Dispose()
        {
            ((IDisposable)_queue).Dispose();
        }
    }
}
