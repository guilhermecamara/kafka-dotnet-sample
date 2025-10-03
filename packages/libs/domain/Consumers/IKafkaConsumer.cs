using System.Threading.Channels;

namespace KafkaDotnetSample.Domain.Consumers;

public interface IKafkaConsumer
{
    ChannelReader<(string Topic, string Data)> Events { get; }
    Task StartAsync(CancellationToken cancellationToken = default);
}
