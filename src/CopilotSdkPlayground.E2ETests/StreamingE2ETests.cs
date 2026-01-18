using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// ストリーミングモードの E2E テスト
/// </summary>
[Collection("CopilotClient")]
public class StreamingE2ETests(CopilotClientFixture fixture)
{
    private readonly CopilotClientFixture _fixture = fixture;

    [Fact]
    public async Task StreamingMode_ShouldReceiveResponse()
    {
        // Skip if not available
        Skip.If(!_fixture.IsAvailable, _fixture.SkipReason);

        // Arrange
        var client = _fixture.Client;
        var session = await client.CreateSessionAsync(new SessionConfig
        {
            Model = "gpt-5",
            Streaming = true
        });

        var receivedContent = new List<string>();
        var done = new TaskCompletionSource();
        var timeout = TimeSpan.FromSeconds(60);

        // Act
        session.On(evt =>
        {
            switch (evt)
            {
                case AssistantMessageDeltaEvent delta:
                    receivedContent.Add(delta.Data.DeltaContent ?? "");
                    break;

                case AssistantMessageEvent msg:
                    receivedContent.Add($"[FINAL]{msg.Data.Content}");
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
        Assert.NotEmpty(receivedContent);
    }

    [Fact]
    public async Task StreamingMode_ShouldReceiveDeltaEvents()
    {
        // Skip if not available
        Skip.If(!_fixture.IsAvailable, _fixture.SkipReason);

        // Arrange
        var client = _fixture.Client;
        var session = await client.CreateSessionAsync(new SessionConfig
        {
            Model = "gpt-5",
            Streaming = true
        });

        var deltaCount = 0;
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

                case SessionIdleEvent:
                    done.SetResult();
                    break;
            }
        });

        await session.SendAsync(new MessageOptions { Prompt = "Write a short sentence about the weather." });

        var completedTask = await Task.WhenAny(done.Task, Task.Delay(timeout));

        // Assert
        Assert.True(completedTask == done.Task, "Response should be received within timeout");
        Assert.True(deltaCount > 0, "Should receive delta events for streaming");
    }
}
