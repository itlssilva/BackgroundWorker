using System.Text;
using BackgroundWorker.Api.Models;
using BackgroundWorker.Api.ClientMq;
using IBM.WMQ;

namespace BackgroundWorker.Api.MqQueueReader;

public class QueueReader : IQueueReader
{
    private readonly IMQClient _mqClient;
    private readonly QueueOptions _queueOptions;
    private readonly MQGetMessageOptions _messageGetOptions = new MQGetMessageOptions();
    private DateTimeOffset LastQueueEmptyWarningReported;
    //private readonly ILoggerManager _logger;
    const int QueueEmptyReportTimeIntervalMinutes = 10;

    public QueueReader(IMQClient mQClient/*, ILoggerManager logger*/, QueueOptions mqOptions)
    {
        _mqClient = mQClient;
        _queueOptions = mqOptions;
        //_logger = logger;
    }

    public QueueMessage Dequque(string messageId)
    {
        QueueMessage queueMessage = null;
        MQQueue destination = null;
        try
        {
            //destination = _mqClient.GetResilientQueue(MQC.MQOO_INPUT_AS_Q_DEF | MQC.MQOO_FAIL_IF_QUIESCING, EParametersQueue.Output);
            destination = _mqClient.GetQueue(_queueOptions.QueueOutPut, (int)EParametersQueue.Output);
            MQMessage receivedMsg = new()
            {
                CorrelationId = Encoding.Default.GetBytes(messageId),
                MessageId = Encoding.Default.GetBytes(messageId)
            };
            _messageGetOptions.WaitInterval = 35000;
            _messageGetOptions.Options = MQC.MQGMO_WAIT;

            destination.Get(receivedMsg, _messageGetOptions);
            queueMessage = ToQueueMessage(receivedMsg);
        }
        catch (MQException ex)
        {
            switch (ex.ReasonCode)
            {
                case MQC.MQRC_HOST_NOT_AVAILABLE or
                     MQC.MQRC_NO_MSG_AVAILABLE or
                     MQC.MQRC_CONNECTION_BROKEN:
                    Console.WriteLine(ex.Message);
                    break;
                default:
                    throw;
            }

            ReportQueueEmpty(ex);
        }
        catch (Exception)
        {
            //_logger.LogError($"Error getting message from IBM MQ {ex.Message}");
            throw;
        }
        finally
        {
            destination?.Close();
            ((IDisposable)destination)?.Dispose();
        }

        return queueMessage;
    }

    public QueueMessage Peek()
    {
        QueueMessage queueMessage = null;
        MQQueue destination = null;
        try
        {
            destination = _mqClient.GetResilientQueue(MQC.MQOO_BROWSE | MQC.MQOO_FAIL_IF_QUIESCING, EParametersQueue.Output);
            MQMessage receivedMsg = new MQMessage();
            MQGetMessageOptions gmo = new MQGetMessageOptions
            {
                Options = MQC.MQGMO_BROWSE_NEXT
            };            

            destination.Get(receivedMsg, gmo);
            queueMessage = ToQueueMessage(receivedMsg);
        }
        catch (MQException ex)
        {
            if (ex.ReasonCode == MQC.MQRC_NO_MSG_AVAILABLE)
            {
                ReportQueueEmpty(ex);
                throw;
            }
            else
            {
                //_logger.LogError($"Error getting message from IBM MQ {ex.Message} {ex.GetErrorCodeDescription()}");
                throw;
            }
        }
        catch (Exception)
        {
            //_logger.LogError($"Error getting message from IBM MQ {ex.Message} ");
            throw;
        }
        finally
        {
            destination?.Close();
            ((IDisposable)destination)?.Dispose();
        }
        return queueMessage;
    }


    #region Helpers
    private QueueMessage ToQueueMessage(MQMessage message)
    {
        QueueMessage resultMessage = null;
        if (message != null)
        {
            resultMessage = new QueueMessage
            {
                Data = message.ReadString(message.MessageLength)
            };
        }
        return resultMessage;
    }

    private void ReportQueueEmpty(MQException ex)
    {
        var timeSpan = DateTimeOffset.UtcNow.Subtract(LastQueueEmptyWarningReported);

        if (timeSpan.TotalMinutes > QueueEmptyReportTimeIntervalMinutes)
        {
            //_logger.LogError($"Queue is empty error getting the message.{ex.Message} {ex.GetErrorCodeDescription()}");
            LastQueueEmptyWarningReported = DateTimeOffset.UtcNow;
        }
    }
    #endregion
}