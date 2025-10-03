using Microsoft.Extensions.Logging;
using KafkaDotnetSample.Domain.Publishers;

namespace KafkaDotnetSample.Domain.UseCases;

public class LogoutRequest
{
    public string UserId { get; set; } = string.Empty;
}

public class LogoutUseCase
{
    private readonly IKafkaPublisher _publisher;
    private readonly ILogger<LogoutUseCase> _logger;
    public LogoutUseCase(IKafkaPublisher publisher, ILogger<LogoutUseCase> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(LogoutRequest req)
    {
        _logger.LogInformation("Processing logout for user: {UserId}", req.UserId);
        try
        {
            await _publisher.PublishAsync("logout", $"{{ \"userId\": \"{req.UserId}\" }}");
            _logger.LogInformation("Published logout event for user: {UserId}", req.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing logout event for user: {UserId}", req.UserId);
        }
        return true;
    }
}
