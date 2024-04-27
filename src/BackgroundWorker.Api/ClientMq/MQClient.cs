using System.Collections;
using BackgroundWorker.Api.Models;
using IBM.WMQ;
using Polly;
using Polly.Retry;

namespace BackgroundWorker.Api.ClientMq;

public class MQClient : IMQClient
{
    private readonly object InstanceLoker = new object();
    private readonly QueueOptions _queueOptions;
    private Hashtable _connectionOptions;
    private MQQueueManager _manager;
    //private readonly ILoggerManager _logger;
    private readonly RetryPolicy _defaultPolicy;

    public MQClient(QueueOptions mqOptions/*, ILoggerManager logger*/)
    {
        _queueOptions = mqOptions;
        // _logger = logger;
        // _logger.LogInformation(_queueOptions.ToString());
        CreateConnectionOptions();

        _defaultPolicy = Policy.Handle<MQException>()
                            .WaitAndRetry(_queueOptions.RetryCount > 0 ? _queueOptions.RetryCount : 5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    #region Helpers
    private MQQueueManager Manager
    {
        get
        {
            if (_manager == null || _manager?.IsConnected == false || _manager?.ReasonCode != MQC.OK)
            {
                lock (InstanceLoker)
                    if (_manager == null || _manager?.IsConnected == false || _manager?.ReasonCode != MQC.OK)
                            GetMqManager();
            }
            return _manager;
        }
    }    

    public MQQueue GetQueue(string queueName, int openOptions)
    {
        MQQueue queue;
        try
        {
            queue = Manager.AccessQueue(queueName, openOptions);
        }
        catch (MQException)
        {
            //_logger.LogError($"Error while trying to access IBM MQ {queueName} - {ex.Message}");
            DisposeQueueManagerConnection();
            throw;
        }
        return queue;
    }

    public MQQueue GetResilientQueue(int openOptions, EParametersQueue parametersQueue)
    {
        var policyResult = _defaultPolicy.ExecuteAndCapture(() => GetQueue(parametersQueue == EParametersQueue.Input
                        ? _queueOptions.QueueInput : _queueOptions.QueueOutPut, openOptions));
        EnsureSuccess(policyResult);
        return policyResult.Result;
    }

    private void EnsureSuccess(PolicyResult<MQQueue> policyResult)
    {
        if (policyResult.Outcome == OutcomeType.Failure && policyResult.FinalException != null)
            throw policyResult.FinalException;
    }

    private MQQueueManager GetMqManager()
    {
        var QManagerName = _queueOptions.ManagerName;
        if (_manager?.IsConnected == true) return _manager;
        //_logger.LogInformation($"Attempting to connect to queue manager: {QManagerName}");

        try
        {
            if(_connectionOptions.Count > 0){
                // foreach (DictionaryEntry item in _connectionOptions)
                //     Console.WriteLine($"Log HashTable: {item.Key} | {item.Value}");
                //_logger.LogInformation($"Log HashTable: {item.Key} | {item.Value}");
            }

            _manager = new MQQueueManager(QManagerName, _connectionOptions);
            //_logger.LogInformation($"Connected to queue manager: {QManagerName}");
        }
        catch (MQException)
        {
            //_logger.LogError($"A WebSphere MQ error occurred while creating Connection to QManager [{QManagerName}] : {ex}");
            throw;
        }
        return _manager;
    }

    private void CreateConnectionOptions()
    {
        if (_queueOptions == null)
            throw new ArgumentException("No IBM MQ config options was found!");

        _connectionOptions = new Hashtable
            {
                { MQC.TRANSPORT_PROPERTY, "TCP" },
                { MQC.CHANNEL_PROPERTY, _queueOptions.Channel },
                { MQC.HOST_NAME_PROPERTY, _queueOptions.MqHostOptionsList.FirstOrDefault().HostName },
                { MQC.PORT_PROPERTY, _queueOptions.MqHostOptionsList.FirstOrDefault().Port },
                //{ MQC.SSL_CIPHER_SUITE_PROPERTY, _queueOptions.Suite },
                //{ MQC.SSL_CIPHER_SPEC_PROPERTY, _queueOptions.Spec },
                //{ MQC.SSL_CERT_STORE_PROPERTY, "*USER" }
            };

        if (_queueOptions.IsTest)
        {
            _connectionOptions.Add(MQC.USER_ID_PROPERTY, $"{_queueOptions.Username}");
            _connectionOptions.Add(MQC.PASSWORD_PROPERTY, $"{_queueOptions.Password}");
        }
    }

    private void DisposeQueueManagerConnection()
    {
        if (_manager?.IsConnected == true)
        {
            //_logger.LogInformation("Disconnecting Queue Manager.");
            _manager?.Disconnect();
            //_logger.LogInformation("Queue Manager disconnected.");
        }
        
        ((IDisposable)_manager)?.Dispose();
        //_logger.LogInformation("Disposed Queue Manager.");
        _manager = null;
    }

    #endregion
}