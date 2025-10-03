using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using KafkaDotnetSample.Domain.Publishers;

namespace KafkaDotnetSample.Infra.Publishers;

public class KafkaPublisher : IKafkaPublisher
{
    private readonly IProducer<Null, string> _producer;
    private readonly ILogger<KafkaPublisher> _logger;
    public KafkaPublisher(IProducer<Null, string> producer, ILogger<KafkaPublisher> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async Task PublishAsync(string topic, string message)
    {
        try
        {
            await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            _logger.LogInformation("Published message to topic: {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to topic: {Topic}", topic);
            throw;
        }
    }
}
