using Xunit;
using Moq;
using BackgroundWorker.Api.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using System;
using System.Threading;

namespace BackgroundWorker.Api.Tests.Kafka
{
    public class ConsumerServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<ConsumerService>> _loggerMock;
        private readonly Mock<IConsumer<Ignore, string>> _consumerMock;
        private readonly ConsumerService _consumerService;

        public ConsumerServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _loggerMock = new Mock<ILogger<ConsumerService>>();
            _consumerMock = new Mock<IConsumer<Ignore, string>>();

            var kafkaConfigSectionMock = new Mock<IConfigurationSection>();
            var topicSectionMock = new Mock<IConfigurationSection>();
            var bootstrapServerSectionMock = new Mock<IConfigurationSection>();

            topicSectionMock.Setup(s => s.Value).Returns("test_topic");
            bootstrapServerSectionMock.Setup(s => s.Value).Returns("test_server");

            kafkaConfigSectionMock.Setup(s => s.GetSection("TopicName")).Returns(topicSectionMock.Object);
            kafkaConfigSectionMock.Setup(s => s.GetSection("BootstrapServer")).Returns(bootstrapServerSectionMock.Object);

            _configurationMock.Setup(c => c.GetSection("KafkaConfig")).Returns(kafkaConfigSectionMock.Object);

            _consumerService = new ConsumerService(
                _configurationMock.Object,
                _loggerMock.Object,
                _consumerMock.Object
            );
        }

        [Fact]
        public void GetMessageAsync_ShouldReturnMessage_WhenMessageIsConsumed()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var expectedMessage = "test message";
            var consumeResult = new ConsumeResult<Ignore, string>
            {
                Message = new Message<Ignore, string> { Value = expectedMessage }
            };

            _consumerMock.Setup(c => c.Consume(cancellationToken)).Returns(consumeResult);

            // Act
            var result = _consumerService.GetMessageAsync(cancellationToken);

            // Assert
            Assert.Equal(expectedMessage, result);
            _consumerMock.Verify(c => c.Subscribe("test_topic"), Times.Once);
        }

        [Fact]
        public void ConsumerServiceClose_ShouldCloseConsumer()
        {
            // Arrange
            // Nothing to arrange

            // Act
            _consumerService.ConsumerServiceClose();

            // Assert
            _consumerMock.Verify(c => c.Close(), Times.Once);
        }
    }
}
