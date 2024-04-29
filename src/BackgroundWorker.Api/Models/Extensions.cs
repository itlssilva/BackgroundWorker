using IBM.WMQ;

namespace BackgroundWorker.Api.Models;

public static class Extensions
{
    public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
    {
        var model = new TModel();
        configuration.GetSection(section).Bind(model);

        return model;
    }

    public static string GetErrorCodeDescription(this MQException mqException)
    {
        string description = "See IBM site for description";

        if (MQErrorDescription.ContainsKey(mqException.ReasonCode) == true)
            description = MQErrorDescription[mqException.ReasonCode].ToString();

        return description;
    }

    public static int[] RetryExceptionCode =>
    [
        MQC.MQRC_CONNECTION_QUIESCING,
        MQC.MQRC_BUFFER_ERROR,
        MQC.MQRC_BUFFER_LENGTH_ERROR,
        MQC.MQRC_CONNECTION_BROKEN,
        MQC.MQRC_SYNCPOINT_LIMIT_REACHED,
        MQC.MQRC_MAX_CONNS_LIMIT_REACHED,
        MQC.MQRC_NOT_AUTHORIZED,
        MQC.MQRC_WAIT_INTERVAL_ERROR,
        MQC.MQRC_RESOURCE_PROBLEM,
        MQC.MQRC_TRUNCATED,
        MQC.MQRC_OPEN_FAILED,
        MQC.MQRC_SOURCE_BUFFER_ERROR,
        MQC.MQRC_TARGET_BUFFER_ERROR,
        MQC.MQRC_Q_MGR_QUIESCING
    ];

    private static readonly Lazy<Dictionary<int, string>> mqErrorDescription = new(() =>
    {
        return new Dictionary<int, string>()
            {
                { MQC.MQRC_NOT_AUTHORIZED, "MQ NOT AUTHORIZED" },
                { MQC.MQRC_UNKNOWN_OBJECT_NAME, "MQ UNKNOWN OBJECT NAME" },
                { MQC.MQRC_NO_MSG_AVAILABLE, "MQ NO MESSAGE AVAILABLE" },
                { MQC.MQRC_HOST_NOT_AVAILABLE, "MQ HOST NOT AVAILABLE" },
                { MQC.MQRC_CONNECTION_QUIESCING, "MQ CONNECTION QUIESCING" }
            };
    }, true);
    
    public static Dictionary<int, string> MQErrorDescription
    {
        get
        {
            return mqErrorDescription.Value;
        }
    }
}