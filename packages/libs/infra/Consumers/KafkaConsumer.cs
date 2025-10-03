using System.Threading.Channels;
using Confluent.Kafka;
using KafkaDotnetSample.Domain.Consumers;
using KafkaDotnetSample.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace KafkaDotnetSample.Infra.Consumers;

public class KafkaConsumer : IKafkaConsumer
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IProcessedEventsRepository _eventsRepository;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly string[] _topics;

    public ChannelReader<(string Topic, string Data)> Events => _eventsRepository.Events;

    public KafkaConsumer(string bootstrapServers, string groupId, IProcessedEventsRepository eventsRepository, ILogger<KafkaConsumer> logger, params string[] topics)
    {
        _topics = topics;
        _eventsRepository = eventsRepository;
        _logger = logger;
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };
        _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _consumer.Subscribe(_topics);
        _logger.LogInformation("KafkaConsumer subscribed to topics: {Topics}", string.Join(",", _topics));
        await Task.Yield(); // Ensure async
        _ = Task.Run(async () =>
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = _consumer.Consume(cancellationToken);
                    if (result != null)
                    {
                        await _eventsRepository.AddEventAsync(result.Topic, result.Message.Value);
                        _logger.LogInformation("Consumed event from topic: {Topic}", result.Topic);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming Kafka event");
            }
            finally
            {
                _consumer.Close();
                _logger.LogInformation("KafkaConsumer closed.");
            }
        }, cancellationToken);
    }
}
