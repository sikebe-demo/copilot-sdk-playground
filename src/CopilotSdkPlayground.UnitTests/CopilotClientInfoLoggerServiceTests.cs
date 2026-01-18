using CopilotSdkPlayground.Abstractions;
using Microsoft.Extensions.Logging;

namespace CopilotSdkPlayground.UnitTests;

public class CopilotClientInfoLoggerServiceTests
{
    private readonly Mock<ILogger<CopilotClientInfoLoggerService>> _loggerMock;
    private readonly Mock<IEnvironmentProvider> _environmentProviderMock;
    private readonly Mock<IFileSystem> _fileSystemMock;
    private readonly CopilotClientInfoLoggerService _sut;

    public CopilotClientInfoLoggerServiceTests()
    {
        _loggerMock = new Mock<ILogger<CopilotClientInfoLoggerService>>();
        _environmentProviderMock = new Mock<IEnvironmentProvider>();
        _fileSystemMock = new Mock<IFileSystem>();
        _sut = new CopilotClientInfoLoggerService(
            _loggerMock.Object,
            _environmentProviderMock.Object,
            _fileSystemMock.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldNotThrow()
    {
        // Arrange & Act
        var service = new CopilotClientInfoLoggerService(
            _loggerMock.Object,
            _environmentProviderMock.Object,
            _fileSystemMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CopilotClientInfoLoggerService(
            null!,
            _environmentProviderMock.Object,
            _fileSystemMock.Object));
    }

    [Fact]
    public void Constructor_WithNullEnvironmentProvider_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CopilotClientInfoLoggerService(
            _loggerMock.Object,
            null!,
            _fileSystemMock.Object));
    }

    [Fact]
    public void Constructor_WithNullFileSystem_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CopilotClientInfoLoggerService(
            _loggerMock.Object,
            _environmentProviderMock.Object,
            null!));
    }
}
