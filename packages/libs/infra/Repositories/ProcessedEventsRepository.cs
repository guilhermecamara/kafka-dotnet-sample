using System.Threading.Channels;
using KafkaDotnetSample.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace KafkaDotnetSample.Infra.Repositories;

public class ProcessedEventsRepository : IProcessedEventsRepository
{
    private readonly Channel<(string Topic, string Data)> _channel = Channel.CreateUnbounded<(string Topic, string Data)>();
    private readonly ILogger<ProcessedEventsRepository> _logger;
    public ProcessedEventsRepository(ILogger<ProcessedEventsRepository> logger)
    {
        _logger = logger;
    }
    public ChannelReader<(string Topic, string Data)> Events => _channel.Reader;
    public async Task AddEventAsync(string topic, string data)
    {
        await _channel.Writer.WriteAsync((topic, data));
        _logger.LogInformation("Event added: {Topic} - {Data}", topic, data);
    }
    public void Complete()
    {
        _channel.Writer.Complete();
    }
}
