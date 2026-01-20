using CopilotSdkPlayground.Infrastructure;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;

namespace CopilotSdkPlayground.UnitTests.Infrastructure;

public class CopilotClientWrapperFactoryTests
{
    [Fact]
    public void Create_WithValidClient_ShouldReturnWrapper()
    {
        // Arrange
        var factory = new CopilotClientWrapperFactory();
        var client = new CopilotClient(new CopilotClientOptions
        {
            Logger = Mock.Of<ILogger<CopilotClient>>()
        });

        // Act
        var wrapper = factory.Create(client);

        // Assert
        Assert.NotNull(wrapper);
    }

    [Fact]
    public void Create_WithNullClient_ShouldThrowArgumentNullException()
    {
        // Arrange
        var factory = new CopilotClientWrapperFactory();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => factory.Create(null!));
    }
}
