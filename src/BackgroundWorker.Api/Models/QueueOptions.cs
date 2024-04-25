namespace BackgroundWorker.Api.Models;

public class QueueOptions
{
    public string QueueInput { get; set; }
    public string QueueOutPut { get; set; }
    public string AppName { get; set; }
    public string ManagerName { get; set; }
    public string Channel { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int RetryCount { get; set; }
    public List<MqHostOptions> MqHostOptionsList { get; set; }
    public string CipherSpec { get; set; }
    public string SslKeyRepository { get; set; }
    public bool IsTest { get; set; }
    public string Spec { get; set; }
    public string Suite { get; set; }
    public string ApplicationIdData { get; set; }

    public override string ToString()
    {
        return @$"
                QueueInput: {QueueInput}
                QueueOutPut: {QueueOutPut}
                AppName: {AppName}
                ManagerName: {ManagerName} 
                Channel: {Channel}
                Username: {Username}
                Password: {Password}
                RetryCount: {RetryCount}
                HostName: {MqHostOptionsList.FirstOrDefault().HostName}
                Port: {MqHostOptionsList.FirstOrDefault().Port}
                CipherSpec: {CipherSpec}
                SslKeyRepository: {SslKeyRepository}
                IsTest: {IsTest}";
    }
}

public class MqHostOptions
{
    public string HostName { get; set; }
    public string Port { get; set; }
}
