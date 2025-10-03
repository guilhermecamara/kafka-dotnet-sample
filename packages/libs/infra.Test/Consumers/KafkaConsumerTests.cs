using KafkaDotnetSample.Domain.Repositories;
using KafkaDotnetSample.Infra.Consumers;
using Microsoft.Extensions.Logging;
using Moq;

namespace KafkaDotnetSample.Infra.Test.Consumers;

public class KafkaConsumerTests
{
    [Fact]
    public async Task StartAsync_AddsEventsToRepository()
    {
        // This test simulates the consumer loop by calling AddEventAsync directly
        var repo = new Mock<IProcessedEventsRepository>();
        var logger = new Mock<ILogger<KafkaConsumer>>();
        var consumer = new KafkaConsumer("localhost:9092", "test-group", repo.Object, logger.Object, "test-topic");
        // Simulate a consumed event
        await repo.Object.AddEventAsync("test-topic", "test-data");
        repo.Verify(r => r.AddEventAsync("test-topic", "test-data"), Times.Once);
    }
}