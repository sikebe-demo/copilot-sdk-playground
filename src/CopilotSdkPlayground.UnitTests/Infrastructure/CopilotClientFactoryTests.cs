using CopilotSdkPlayground.Infrastructure;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.UnitTests.Infrastructure;

public class CopilotClientFactoryTests
{
    private readonly CopilotClientFactory _sut;

    public CopilotClientFactoryTests()
    {
        _sut = new CopilotClientFactory();
    }

    [Fact]
    public void Constructor_ShouldNotThrow()
    {
        // Arrange & Act & Assert
        var factory = new CopilotClientFactory();
        Assert.NotNull(factory);
    }

    [Fact]
    public void Create_WithValidOptions_ShouldReturnCopilotClient()
    {
        // Arrange
        var options = new CopilotClientOptions();

        // Act
        var client = _sut.Create(options);

        // Assert
        Assert.NotNull(client);
        Assert.IsType<CopilotClient>(client);
    }
}
