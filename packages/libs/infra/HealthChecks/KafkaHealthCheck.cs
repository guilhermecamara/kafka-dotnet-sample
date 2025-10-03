using Microsoft.Extensions.Diagnostics.HealthChecks;
using Confluent.Kafka;

namespace KafkaDotnetSample.Infra.HealthChecks;

public class KafkaHealthCheck : IHealthCheck
{
    private readonly string _bootstrapServers;
    public KafkaHealthCheck(string bootstrapServers)
    {
        _bootstrapServers = bootstrapServers;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var config = new AdminClientConfig { BootstrapServers = _bootstrapServers };
            using var adminClient = new AdminClientBuilder(config).Build();
            // Try to get cluster metadata (list topics) to verify connection
            var meta = adminClient.GetMetadata(TimeSpan.FromSeconds(2));
            if (meta.Topics != null && meta.Topics.Count > 0)
                return HealthCheckResult.Healthy("Kafka connection successful");
            return HealthCheckResult.Degraded("Kafka connected, but no topics found");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Kafka connection failed: {ex.Message}");
        }
    }
}
