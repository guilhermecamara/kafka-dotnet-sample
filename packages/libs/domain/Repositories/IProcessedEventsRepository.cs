using System.Threading.Channels;

namespace KafkaDotnetSample.Domain.Repositories;

public interface IProcessedEventsRepository
{
    ChannelReader<(string Topic, string Data)> Events { get; }
    Task AddEventAsync(string topic, string data);
}
