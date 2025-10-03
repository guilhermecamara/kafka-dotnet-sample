using Microsoft.Extensions.Logging;
using KafkaDotnetSample.Domain.Publishers;

namespace KafkaDotnetSample.Domain.UseCases;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginUseCase
{
    private readonly IKafkaPublisher _publisher;
    private readonly ILogger<LoginUseCase> _logger;
    public LoginUseCase(IKafkaPublisher publisher, ILogger<LoginUseCase> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(LoginRequest req)
    {
        var result = req.Username == "user" && req.Password == "pass";
        _logger.LogInformation("Processing login for user: {Username}", req.Username);
        try
        {
            await _publisher.PublishAsync("login", $"{{ \"username\": \"{req.Username}\", \"success\": {result.ToString().ToLower()} }}");
            _logger.LogInformation("Published login event for user: {Username}", req.Username);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing login event for user: {Username}", req.Username);
        }
        return result;
    }
}
