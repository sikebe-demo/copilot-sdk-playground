using CopilotSdkPlayground.Abstractions;
using CopilotSdkPlayground.Demos;
using CopilotSdkPlayground.UnitTests.TestHelpers;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.UnitTests;

public class NonStreamingDemoServiceTests
{
    private readonly Mock<IConsoleWriter> _consoleWriterMock;
    private readonly NonStreamingDemoService _sut;

    public NonStreamingDemoServiceTests()
    {
        _consoleWriterMock = new Mock<IConsoleWriter>();
        _sut = new NonStreamingDemoService(_consoleWriterMock.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldNotThrow()
    {
        // Arrange & Act
        var service = new NonStreamingDemoService(_consoleWriterMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullConsoleWriter_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new NonStreamingDemoService(null!));
    }

    [Fact]
    public async Task RunAsync_ShouldDisplayDemoHeader()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();

        SessionEventHandler? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<SessionEventHandler>()))
            .Callback<SessionEventHandler>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);

        // 非同期でイベントを発火させてタスクを完了させる
        await Task.Delay(50);
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.WriteLine("=== Non-Streaming Mode Demo ==="), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_ShouldCreateSessionWithCorrectConfig()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();
        SessionConfig? capturedConfig = null;

        SessionEventHandler? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<SessionEventHandler>()))
            .Callback<SessionEventHandler>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .Callback<SessionConfig>(config => capturedConfig = config)
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);
        await Task.Delay(50);
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        Assert.NotNull(capturedConfig);
        Assert.Equal("gpt-5", capturedConfig.Model);
        Assert.False(capturedConfig.Streaming);
    }

    [Fact]
    public async Task RunAsync_WhenAssistantMessageEventReceived_ShouldDisplayMessage()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();

        SessionEventHandler? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<SessionEventHandler>()))
            .Callback<SessionEventHandler>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);
        await Task.Delay(50);

        // メッセージイベントを発火
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageEvent("Test message"));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.WriteLine("Test message"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_WhenAssistantReasoningEventReceived_ShouldDisplayReasoning()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();

        SessionEventHandler? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<SessionEventHandler>()))
            .Callback<SessionEventHandler>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);
        await Task.Delay(50);

        // 推論イベントを発火
        capturedHandler?.Invoke(EventFactory.CreateAssistantReasoningEvent("Reasoning content"));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.WriteLine("--- Reasoning ---"), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine("Reasoning content"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_ShouldSendCorrectPrompt()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();
        MessageOptions? capturedOptions = null;

        SessionEventHandler? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<SessionEventHandler>()))
            .Callback<SessionEventHandler>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Callback<MessageOptions>(options => capturedOptions = options)
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);
        await Task.Delay(50);
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        Assert.NotNull(capturedOptions);
        Assert.Equal("小話をして", capturedOptions.Prompt);
    }

    [Fact]
    public async Task RunAsync_ShouldDisplayAssistantLabel()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();

        SessionEventHandler? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<SessionEventHandler>()))
            .Callback<SessionEventHandler>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);
        await Task.Delay(50);
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.WriteLine("Assistant: "), Times.Once);
    }
}
