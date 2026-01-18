using CopilotSdkPlayground.Abstractions;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;

namespace CopilotSdkPlayground.UnitTests;

public class AppTests
{
    private readonly Mock<ICopilotClientFactory> _clientFactoryMock;
    private readonly Mock<IStreamingDemo> _streamingDemoMock;
    private readonly Mock<INonStreamingDemo> _nonStreamingDemoMock;
    private readonly Mock<ICopilotClientInfoLogger> _clientInfoLoggerMock;
    private readonly Mock<ILogger<App>> _loggerMock;
    private readonly Mock<ILogger<CopilotClient>> _copilotLoggerMock;

    public AppTests()
    {
        _clientFactoryMock = new Mock<ICopilotClientFactory>();
        _streamingDemoMock = new Mock<IStreamingDemo>();
        _nonStreamingDemoMock = new Mock<INonStreamingDemo>();
        _clientInfoLoggerMock = new Mock<ICopilotClientInfoLogger>();
        _loggerMock = new Mock<ILogger<App>>();
        _copilotLoggerMock = new Mock<ILogger<CopilotClient>>();
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldNotThrow()
    {
        // Arrange & Act
        var app = CreateApp();

        // Assert
        Assert.NotNull(app);
    }

    [Theory]
    [InlineData(new string[] { }, false)]
    [InlineData(new string[] { "--no-streaming" }, true)]
    [InlineData(new string[] { "--NO-STREAMING" }, true)]
    [InlineData(new string[] { "--No-Streaming" }, true)]
    [InlineData(new string[] { "--streaming" }, false)]
    [InlineData(new string[] { "other", "--no-streaming" }, true)]
    public void IsNoStreamingMode_WithVariousArgs_ShouldReturnExpectedResult(string[] args, bool expected)
    {
        // Act
        var result = App.IsNoStreamingMode(args);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Constructor_WithNullClientFactory_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new App(
            null!,
            _streamingDemoMock.Object,
            _nonStreamingDemoMock.Object,
            _clientInfoLoggerMock.Object,
            _loggerMock.Object,
            _copilotLoggerMock.Object));
    }

    [Fact]
    public void Constructor_WithNullStreamingDemo_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new App(
            _clientFactoryMock.Object,
            null!,
            _nonStreamingDemoMock.Object,
            _clientInfoLoggerMock.Object,
            _loggerMock.Object,
            _copilotLoggerMock.Object));
    }

    [Fact]
    public void Constructor_WithNullNonStreamingDemo_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new App(
            _clientFactoryMock.Object,
            _streamingDemoMock.Object,
            null!,
            _clientInfoLoggerMock.Object,
            _loggerMock.Object,
            _copilotLoggerMock.Object));
    }

    [Fact]
    public void Constructor_WithNullClientInfoLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new App(
            _clientFactoryMock.Object,
            _streamingDemoMock.Object,
            _nonStreamingDemoMock.Object,
            null!,
            _loggerMock.Object,
            _copilotLoggerMock.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new App(
            _clientFactoryMock.Object,
            _streamingDemoMock.Object,
            _nonStreamingDemoMock.Object,
            _clientInfoLoggerMock.Object,
            null!,
            _copilotLoggerMock.Object));
    }

    [Fact]
    public void Constructor_WithNullCopilotLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new App(
            _clientFactoryMock.Object,
            _streamingDemoMock.Object,
            _nonStreamingDemoMock.Object,
            _clientInfoLoggerMock.Object,
            _loggerMock.Object,
            null!));
    }

    private App CreateApp()
    {
        return new App(
            _clientFactoryMock.Object,
            _streamingDemoMock.Object,
            _nonStreamingDemoMock.Object,
            _clientInfoLoggerMock.Object,
            _loggerMock.Object,
            _copilotLoggerMock.Object);
    }
}
