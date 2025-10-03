using Microsoft.Extensions.Logging;
using KafkaDotnetSample.Domain.Publishers;

namespace KafkaDotnetSample.Domain.UseCases;

public class PurchaseRequest
{
    public string UserId { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public int Amount { get; set; }
}

public class PurchaseUseCase
{
    private readonly IKafkaPublisher _publisher;
    private readonly ILogger<PurchaseUseCase> _logger;
    public PurchaseUseCase(IKafkaPublisher publisher, ILogger<PurchaseUseCase> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(PurchaseRequest req)
    {
        var result = req.Amount > 0;
        _logger.LogInformation("Processing purchase for user: {UserId}, product: {ProductId}, amount: {Amount}", req.UserId, req.ProductId, req.Amount);
        try
        {
            await _publisher.PublishAsync("purchase", $"{{ \"userId\": \"{req.UserId}\", \"productId\": \"{req.ProductId}\", \"amount\": {req.Amount}, \"success\": {result.ToString().ToLower()} }}");
            _logger.LogInformation("Published purchase event for user: {UserId}, product: {ProductId}", req.UserId, req.ProductId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing purchase event for user: {UserId}, product: {ProductId}", req.UserId, req.ProductId);
        }
        return result;
    }
}
