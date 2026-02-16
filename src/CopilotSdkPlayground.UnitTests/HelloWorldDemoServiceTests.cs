using CopilotSdkPlayground.Abstractions;
using CopilotSdkPlayground.Demos;
using CopilotSdkPlayground.UnitTests.TestHelpers;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.UnitTests;

/// <summary>
/// HelloWorldDemoService のユニットテスト
/// </summary>
/// <remarks>
/// タイムアウト処理のテストは <see cref="Helpers.TaskCompletionSourceExtensionsTests"/> でカバーされています。
/// - SessionIdleEvent が発生しない場合のタイムアウト動作
/// - TimeoutException のメッセージと型
/// - CancellationTokenSource の適切な破棄
/// </remarks>
public class HelloWorldDemoServiceTests
{
    private readonly Mock<IConsoleWriter> _consoleWriterMock;
    private readonly HelloWorldDemoService _sut;

    public HelloWorldDemoServiceTests()
    {
        _consoleWriterMock = new Mock<IConsoleWriter>();
        _sut = new HelloWorldDemoService(_consoleWriterMock.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldNotThrow()
    {
        // Arrange & Act
        var service = new HelloWorldDemoService(_consoleWriterMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullConsoleWriter_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new HelloWorldDemoService(null!));
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
        await Task.Delay(50);
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.WriteLine("=== Hello World Demo ==="), Times.Once);
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
        Assert.True(capturedConfig.Streaming);
    }

    [Fact]
    public async Task RunAsync_WhenAssistantMessageDeltaEventReceived_ShouldDisplayDelta()
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
    public async Task RunAsync_WhenAssistantMessageEventReceived_ShouldDisplayContentAndNewLine()
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
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageEvent("Final content"));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert - メッセージ内容と改行が表示されることを確認
        _consoleWriterMock.Verify(c => c.WriteLine("Final content"), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine(), Times.AtLeast(2)); // ヘッダー後とメッセージ後
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
        Assert.Equal("Hello, Copilot! Please introduce yourself in one sentence.", capturedOptions.Prompt);
    }

    [Fact]
    public async Task RunAsync_ShouldDisplayCopilotLabel()
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
        _consoleWriterMock.Verify(c => c.Write("Copilot says: "), Times.Once);
    }

    [Fact]
    public async Task RunAsync_ShouldHandleMultipleEventsInSequence()
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

        // 複数のイベントを順次発火
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent("I am"));
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent(" Copilot"));
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageEvent("I am Copilot"));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.Write("I am"), Times.Once);
        _consoleWriterMock.Verify(c => c.Write(" Copilot"), Times.Once);
    }
}
