using System.Threading.Channels;
using KafkaDotnetSample.Domain.Repositories;
using KafkaDotnetSample.Domain.UseCases;
using Microsoft.Extensions.Logging;
using Moq;

namespace KafkaDotnetSample.Domain.Test.UseCases;

public class GetProcessedEventsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsAllEvents()
    {
        var channel = Channel.CreateUnbounded<(string, string)>();
        await channel.Writer.WriteAsync(("topic1", "data1"));
        await channel.Writer.WriteAsync(("topic2", "data2"));
        channel.Writer.Complete();
        var repo = new Mock<IProcessedEventsRepository>();
        repo.Setup(r => r.Events).Returns(channel.Reader);
        var logger = new Mock<ILogger<GetProcessedEventsUseCase>>();
        var useCase = new GetProcessedEventsUseCase(repo.Object, logger.Object);
        var events = await useCase.ExecuteAsync();
        Assert.Equal(2, events.Count);
        Assert.Contains(events, e => e.Topic == "topic1" && e.Data == "data1");
        Assert.Contains(events, e => e.Topic == "topic2" && e.Data == "data2");
    }
}