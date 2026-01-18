using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// 非ストリーミングモードの E2E テスト
/// </summary>
[Collection("CopilotClient")]
public class NonStreamingE2ETests(CopilotClientFixture fixture)
{
    private readonly CopilotClientFixture _fixture = fixture;

    [Fact]
    public async Task NonStreamingMode_ShouldReceiveCompleteResponse()
    {
        // Skip if not available
        Skip.If(!_fixture.IsAvailable, _fixture.SkipReason);

        // Arrange
        var client = _fixture.Client;
        var session = await client.CreateSessionAsync(new SessionConfig
        {
            Model = "gpt-5",
            Streaming = false
        });

        string? receivedMessage = null;
        var done = new TaskCompletionSource();
        var timeout = TimeSpan.FromSeconds(60);

        // Act
        session.On(evt =>
        {
            switch (evt)
            {
                case AssistantMessageEvent msg:
                    receivedMessage = msg.Data.Content;
                    break;

                case SessionIdleEvent:
                    done.SetResult();
                    break;
            }
        });

        await session.SendAsync(new MessageOptions { Prompt = "Say hello in one word." });

        var completedTask = await Task.WhenAny(done.Task, Task.Delay(timeout));

        // Assert
        Assert.True(completedTask == done.Task, "Response should be received within timeout");
        Assert.False(string.IsNullOrEmpty(receivedMessage), "Should receive a complete message");
    }

    [Fact]
    public async Task NonStreamingMode_ShouldNotReceiveDeltaEvents()
    {
        // Skip if not available
        Skip.If(!_fixture.IsAvailable, _fixture.SkipReason);

        // Arrange
        var client = _fixture.Client;
        var session = await client.CreateSessionAsync(new SessionConfig
        {
            Model = "gpt-5",
            Streaming = false
        });

        var deltaCount = 0;
        var messageCount = 0;
        var done = new TaskCompletionSource();
        var timeout = TimeSpan.FromSeconds(60);

        // Act
        session.On(evt =>
        {
            switch (evt)
            {
                case AssistantMessageDeltaEvent:
                    Interlocked.Increment(ref deltaCount);
                    break;

                case AssistantMessageEvent:
                    Interlocked.Increment(ref messageCount);
                    break;

                case SessionIdleEvent:
                    done.SetResult();
                    break;
            }
        });

        await session.SendAsync(new MessageOptions { Prompt = "Say hello in one word." });

        var completedTask = await Task.WhenAny(done.Task, Task.Delay(timeout));

        // Assert
        Assert.True(completedTask == done.Task, "Response should be received within timeout");
        Assert.Equal(0, deltaCount);
        Assert.True(messageCount > 0, "Should receive at least one complete message");
    }
}
