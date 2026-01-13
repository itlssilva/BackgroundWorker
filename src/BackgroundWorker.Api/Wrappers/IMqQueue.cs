using IBM.WMQ;

namespace BackgroundWorker.Api.Wrappers
{
    public interface IMqQueue : IDisposable
    {
        void Get(MQMessage message, MQGetMessageOptions options);
        void Close();
    }
}
