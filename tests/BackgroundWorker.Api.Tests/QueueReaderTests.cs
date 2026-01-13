using Xunit;
using Moq;
using BackgroundWorker.Api.MqQueueReader;
using BackgroundWorker.Api.Models;
using BackgroundWorker.Api.Wrappers;
using IBM.WMQ;
using System.Text;

namespace BackgroundWorker.Api.Tests;

public class QueueReaderTests
{
    private readonly Mock<IMqClientWrapper> _mqClientWrapperMock;
    private readonly Mock<IMqQueue> _mqQueueMock;
    private readonly QueueOptions _queueOptions;
    private readonly QueueReader _queueReader;

    public QueueReaderTests()
    {
        _mqClientWrapperMock = new Mock<IMqClientWrapper>();
        _mqQueueMock = new Mock<IMqQueue>();
        _queueOptions = new QueueOptions { QueueOutPut = "TEST_QUEUE" };
        
        _queueReader = new QueueReader(_mqClientWrapperMock.Object, _queueOptions);
    }

    [Fact]
    public void Dequeue_ShouldReturnQueueMessage_WhenMessageExists()
    {
        // Arrange
        var messageId = "test-message-id";
        var messageData = "Hello, World!";
        var messageBytes = Encoding.Default.GetBytes(messageData);

        _mqClientWrapperMock.Setup(c => c.GetQueue(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(_mqQueueMock.Object);

        _mqQueueMock.Setup(q => q.Get(It.IsAny<MQMessage>(), It.IsAny<MQGetMessageOptions>()))
            .Callback<MQMessage, MQGetMessageOptions>((msg, gmo) =>
            {
                msg.ClearMessage();
                msg.Write(messageBytes, 0, messageBytes.Length);
                msg.DataOffset = 0;
                msg.CorrelationId = Encoding.Default.GetBytes(messageId);
            });

        // Act
        var result = _queueReader.Dequque(messageId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(messageData, result.Data);
    }
    
    [Fact]
    public void Dequeue_ShouldReturnNull_WhenNoMessageExists()
    {
        // Arrange
        var messageId = "test-message-id";

        _mqClientWrapperMock.Setup(c => c.GetQueue(It.IsAny<string>(), It.IsAny<int>()))
            .Returns(_mqQueueMock.Object);

        _mqQueueMock.Setup(q => q.Get(It.IsAny<MQMessage>(), It.IsAny<MQGetMessageOptions>()))
            .Throws(new MQException(MQC.MQCC_FAILED, MQC.MQRC_NO_MSG_AVAILABLE));

        // Act
        var result = _queueReader.Dequque(messageId);

        // Assert
        Assert.Null(result);
    }
}
