using Confluent.Kafka;
using KafkaDotnetSample.Infra.Publishers;
using Microsoft.Extensions.Logging;
using Moq;

namespace KafkaDotnetSample.Infra.Test.Publishers;

public class KafkaPublisherTests
{
    [Fact]
    public async Task PublishAsync_CallsProducer()
    {
        var producer = new Mock<IProducer<Null, string>>();
        producer.Setup(p => p.ProduceAsync(
                "topic",
                It.IsAny<Message<Null, string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DeliveryResult<Null, string>());
        var logger = new Mock<ILogger<KafkaPublisher>>();
        var publisher = new KafkaPublisher(producer.Object, logger.Object);
        await publisher.PublishAsync("topic", "data");
        producer.Verify(p => p.ProduceAsync(
            "topic",
            It.IsAny<Message<Null, string>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}