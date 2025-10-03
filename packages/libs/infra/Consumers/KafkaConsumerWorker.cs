using Microsoft.Extensions.Hosting;
using KafkaDotnetSample.Domain.Consumers;

namespace KafkaDotnetSample.Infra.Consumers;

public class KafkaConsumerWorker(IKafkaConsumer consumer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await consumer.StartAsync(stoppingToken);
    }
}

