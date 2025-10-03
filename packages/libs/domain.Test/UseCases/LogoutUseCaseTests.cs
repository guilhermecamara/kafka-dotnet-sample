using KafkaDotnetSample.Domain.Publishers;
using KafkaDotnetSample.Domain.UseCases;
using Microsoft.Extensions.Logging;
using Moq;

namespace KafkaDotnetSample.Domain.Test.UseCases;

public class LogoutUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_PublishesLogoutEvent()
    {
        var publisher = new Mock<IKafkaPublisher>();
        var logger = new Mock<ILogger<LogoutUseCase>>();
        var useCase = new LogoutUseCase(publisher.Object, logger.Object);
        var req = new LogoutRequest { UserId = "u1" };
        var result = await useCase.ExecuteAsync(req);
        Assert.True(result);
        publisher.Verify(p => p.PublishAsync("logout", It.IsAny<string>()), Times.Once);
    }
}