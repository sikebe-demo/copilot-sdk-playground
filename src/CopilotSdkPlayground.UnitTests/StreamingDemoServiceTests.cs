using CopilotSdkPlayground.Abstractions;
using CopilotSdkPlayground.Demos;
using CopilotSdkPlayground.UnitTests.TestHelpers;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.UnitTests;

public class StreamingDemoServiceTests
{
    private readonly Mock<IConsoleWriter> _consoleWriterMock;
    private readonly StreamingDemoService _sut;

    public StreamingDemoServiceTests()
    {
        _consoleWriterMock = new Mock<IConsoleWriter>();
        _sut = new StreamingDemoService(_consoleWriterMock.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldNotThrow()
    {
        // Arrange & Act
        var service = new StreamingDemoService(_consoleWriterMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullConsoleWriter_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new StreamingDemoService(null!));
    }

    [Fact]
    public async Task RunAsync_ShouldDisplayDemoHeader()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();

        Action<object>? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<Action<object>>()))
            .Callback<Action<object>>(handler => capturedHandler = handler);
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
        _consoleWriterMock.Verify(c => c.WriteLine("=== Streaming Mode Demo ==="), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_ShouldCreateSessionWithCorrectConfig()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();
        SessionConfig? capturedConfig = null;

        Action<object>? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<Action<object>>()))
            .Callback<Action<object>>(handler => capturedHandler = handler);
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
        Assert.True(capturedConfig.Streaming);
    }

    [Fact]
    public async Task RunAsync_WhenAssistantMessageDeltaEventReceived_ShouldDisplayDelta()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();

        Action<object>? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<Action<object>>()))
            .Callback<Action<object>>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);
        await Task.Delay(50);

        // デルタイベントを発火
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent("Hello"));
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent(" World"));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.Write("Hello"), Times.Once);
        _consoleWriterMock.Verify(c => c.Write(" World"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_WhenAssistantReasoningDeltaEventReceived_ShouldDisplayReasoningDelta()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();

        Action<object>? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<Action<object>>()))
            .Callback<Action<object>>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);
        await Task.Delay(50);

        // 推論デルタイベントを発火
        capturedHandler?.Invoke(EventFactory.CreateAssistantReasoningDeltaEvent("Thinking..."));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.Write("Thinking..."), Times.Once);
    }

    [Fact]
    public async Task RunAsync_WhenAssistantMessageEventReceived_ShouldDisplayFinalMessage()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();

        Action<object>? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<Action<object>>()))
            .Callback<Action<object>>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);
        await Task.Delay(50);

        // メッセージイベントを発火
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageEvent("Final content"));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.WriteLine("--- Final message ---"), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine("Final content"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_WhenAssistantReasoningEventReceived_ShouldDisplayReasoning()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();

        Action<object>? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<Action<object>>()))
            .Callback<Action<object>>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);
        await Task.Delay(50);

        // 推論イベントを発火
        capturedHandler?.Invoke(EventFactory.CreateAssistantReasoningEvent("Reasoning result"));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.WriteLine("--- Reasoning ---"), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine("Reasoning result"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_ShouldSendCorrectPrompt()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();
        MessageOptions? capturedOptions = null;

        Action<object>? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<Action<object>>()))
            .Callback<Action<object>>(handler => capturedHandler = handler);
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

        Action<object>? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<Action<object>>()))
            .Callback<Action<object>>(handler => capturedHandler = handler);
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

    [Fact]
    public async Task RunAsync_ShouldHandleMultipleEventsInSequence()
    {
        // Arrange
        var sessionMock = new Mock<ICopilotSession>();
        var clientMock = new Mock<ICopilotClientWrapper>();

        Action<object>? capturedHandler = null;
        sessionMock.Setup(s => s.On(It.IsAny<Action<object>>()))
            .Callback<Action<object>>(handler => capturedHandler = handler);
        sessionMock.Setup(s => s.SendAsync(It.IsAny<MessageOptions>()))
            .Returns(Task.CompletedTask);

        clientMock.Setup(c => c.CreateSessionAsync(It.IsAny<SessionConfig>()))
            .ReturnsAsync(sessionMock.Object);

        // Act
        var runTask = _sut.RunAsync(clientMock.Object);
        await Task.Delay(50);

        // 複数のイベントを順次発火
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent("Part1"));
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent("Part2"));
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageEvent("Part1Part2"));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.Write("Part1"), Times.Once);
        _consoleWriterMock.Verify(c => c.Write("Part2"), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine("--- Final message ---"), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine("Part1Part2"), Times.Once);
    }
}
