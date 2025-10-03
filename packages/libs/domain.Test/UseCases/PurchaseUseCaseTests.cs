using KafkaDotnetSample.Domain.Publishers;
using KafkaDotnetSample.Domain.UseCases;
using Microsoft.Extensions.Logging;
using Moq;

namespace KafkaDotnetSample.Domain.Test.UseCases;

public class PurchaseUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsTrue_ForPositiveAmount()
    {
        var publisher = new Mock<IKafkaPublisher>();
        var logger = new Mock<ILogger<PurchaseUseCase>>();
        var useCase = new PurchaseUseCase(publisher.Object, logger.Object);
        var req = new PurchaseRequest { UserId = "u1", ProductId = "p1", Amount = 1 };
        var result = await useCase.ExecuteAsync(req);
        Assert.True(result);
        publisher.Verify(p => p.PublishAsync("purchase", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsFalse_ForZeroAmount()
    {
        var publisher = new Mock<IKafkaPublisher>();
        var logger = new Mock<ILogger<PurchaseUseCase>>();
        var useCase = new PurchaseUseCase(publisher.Object, logger.Object);
        var req = new PurchaseRequest { UserId = "u1", ProductId = "p1", Amount = 0 };
        var result = await useCase.ExecuteAsync(req);
        Assert.False(result);
        publisher.Verify(p => p.PublishAsync("purchase", It.IsAny<string>()), Times.Once);
    }
}