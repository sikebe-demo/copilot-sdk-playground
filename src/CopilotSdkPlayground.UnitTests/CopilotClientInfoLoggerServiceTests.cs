using Microsoft.Extensions.Logging;

namespace CopilotSdkPlayground.UnitTests;

public class CopilotClientInfoLoggerServiceTests
{
    private readonly Mock<ILogger<CopilotClientInfoLoggerService>> _loggerMock;
    private readonly CopilotClientInfoLoggerService _sut;

    public CopilotClientInfoLoggerServiceTests()
    {
        _loggerMock = new Mock<ILogger<CopilotClientInfoLoggerService>>();
        _sut = new CopilotClientInfoLoggerService(_loggerMock.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldNotThrow()
    {
        // Arrange & Act
        var service = new CopilotClientInfoLoggerService(_loggerMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CopilotClientInfoLoggerService(null!));
    }
}
