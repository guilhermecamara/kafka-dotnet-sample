namespace KafkaDotnetSample.Infra.Test.Repositories;

using Xunit;
using KafkaDotnetSample.Infra.Repositories;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;

public class ProcessedEventsRepositoryTests
{
    [Fact]
    public async Task AddEventAsync_StoresEvent()
    {
        var logger = new Moq.Mock<ILogger<ProcessedEventsRepository>>();
        var repo = new ProcessedEventsRepository(logger.Object);
        await repo.AddEventAsync("topic1", "data1");
        await repo.AddEventAsync("topic2", "data2");
        repo.Complete(); // Signal completion so enumeration finishes
        var events = new List<(string Topic, string Data)>();
        await foreach (var evt in repo.Events.ReadAllAsync())
        {
            events.Add(evt);
        }
        Assert.Contains(events, e => e.Topic == "topic1" && e.Data == "data1");
        Assert.Contains(events, e => e.Topic == "topic2" && e.Data == "data2");
    }
}
