using CopilotSdkPlayground.Abstractions;
using CopilotSdkPlayground.Demos;
using CopilotSdkPlayground.UnitTests.TestHelpers;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.UnitTests;

public class ResearchDemoServiceTests
{
    private readonly Mock<IConsoleWriter> _consoleWriterMock;
    private readonly ResearchDemoService _sut;

    public ResearchDemoServiceTests()
    {
        _consoleWriterMock = new Mock<IConsoleWriter>();
        _sut = new ResearchDemoService(_consoleWriterMock.Object);
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldNotThrow()
    {
        // Arrange & Act
        var service = new ResearchDemoService(_consoleWriterMock.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullConsoleWriter_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ResearchDemoService(null!));
    }

    [Fact]
    public void SystemPrompt_ShouldContainResearchCoordinatorRole()
    {
        // Assert
        Assert.Contains("research coordinator", ResearchDemoService.SystemPrompt);
    }

    [Fact]
    public void SystemPrompt_ShouldContainOutputFormat()
    {
        // Assert
        Assert.Contains("Output Format", ResearchDemoService.SystemPrompt);
        Assert.Contains("Topic Analysis", ResearchDemoService.SystemPrompt);
        Assert.Contains("Subtopics", ResearchDemoService.SystemPrompt);
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
        _consoleWriterMock.Verify(c => c.WriteLine("=== Research Agent Demo ==="), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task RunAsync_ShouldDisplayDemoDescription()
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
        _consoleWriterMock.Verify(c => c.WriteLine("This demo shows a research coordinator that can help you"), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine("research any topic by breaking it into subtopics and"), Times.Once);
        _consoleWriterMock.Verify(c => c.WriteLine("providing structured findings."), Times.Once);
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
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent("I am"));
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent(" a research"));
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent(" coordinator"));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.Write("I am"), Times.Once);
        _consoleWriterMock.Verify(c => c.Write(" a research"), Times.Once);
        _consoleWriterMock.Verify(c => c.Write(" coordinator"), Times.Once);
    }

    [Fact]
    public async Task RunAsync_WhenAssistantMessageEventReceived_ShouldAddNewLine()
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

        // Assert - メッセージ完了時に改行が追加されることを確認
        // ヘッダー後、説明後のいくつかの空行、メッセージ後 = 少なくとも3回
        _consoleWriterMock.Verify(c => c.WriteLine(), Times.AtLeast(3));
    }

    [Fact]
    public async Task RunAsync_ShouldSendPromptWithSystemPromptContext()
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
        Assert.Contains("research coordinator", capturedOptions.Prompt);
        Assert.Contains("introduce yourself", capturedOptions.Prompt);
    }

    [Fact]
    public async Task RunAsync_ShouldDisplayResearchAgentLabel()
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
        _consoleWriterMock.Verify(c => c.Write("Research Agent: "), Times.Once);
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
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent("I help with"));
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageDeltaEvent(" research"));
        capturedHandler?.Invoke(EventFactory.CreateAssistantMessageEvent("I help with research"));
        capturedHandler?.Invoke(EventFactory.CreateSessionIdleEvent());
        await runTask;

        // Assert
        _consoleWriterMock.Verify(c => c.Write("I help with"), Times.Once);
        _consoleWriterMock.Verify(c => c.Write(" research"), Times.Once);
    }
}
