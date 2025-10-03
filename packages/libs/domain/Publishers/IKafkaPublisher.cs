namespace KafkaDotnetSample.Domain.Publishers;

public interface IKafkaPublisher
{
    Task PublishAsync(string topic, string message);
}
