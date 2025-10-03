using KafkaDotnetSample.Domain.Publishers;
using KafkaDotnetSample.Domain.UseCases;
using Microsoft.Extensions.Logging;
using Moq;

namespace KafkaDotnetSample.Domain.Test.UseCases;

public class LoginUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsTrue_ForValidCredentials()
    {
        var publisher = new Mock<IKafkaPublisher>();
        var logger = new Mock<ILogger<LoginUseCase>>();
        var useCase = new LoginUseCase(publisher.Object, logger.Object);
        var req = new LoginRequest { Username = "user", Password = "pass" };
        var result = await useCase.ExecuteAsync(req);
        Assert.True(result);
        publisher.Verify(p => p.PublishAsync("login", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsFalse_ForInvalidCredentials()
    {
        var publisher = new Mock<IKafkaPublisher>();
        var logger = new Mock<ILogger<LoginUseCase>>();
        var useCase = new LoginUseCase(publisher.Object, logger.Object);
        var req = new LoginRequest { Username = "bad", Password = "bad" };
        var result = await useCase.ExecuteAsync(req);
        Assert.False(result);
        publisher.Verify(p => p.PublishAsync("login", It.IsAny<string>()), Times.Once);
    }
}