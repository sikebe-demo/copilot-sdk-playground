namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// Hello World デモの E2E テスト
/// ビルド済みアプリケーションをプロセスとして実行します
/// </summary>
[Collection("Process")]
public class HelloWorldE2ETests(ProcessFixture fixture, ITestOutputHelper output)
{
    private readonly ProcessFixture _fixture = fixture;
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task HelloWorldMode_ShouldRunSuccessfullyWithExpectedOutput()
    {
        // Skip if not available
        if (!_fixture.IsAvailable)
        {
            Assert.Skip(_fixture.SkipReason ?? "Test is not available.");
        }

        // Arrange
        var args = new[] { "--hello-world" };

        // Act
        var result = await _fixture.RunApplicationAsync(args);
        result.LogTo(_output);

        // Assert - 正常終了
        Assert.False(result.TimedOut, $"Process should not timeout. StdErr: {result.StandardError}");
        Assert.Equal(0, result.ExitCode);

        // Assert - 期待される出力
        Assert.Contains("=== Hello World Demo ===", result.StandardOutput);
        Assert.Contains("Copilot says:", result.StandardOutput);
        Assert.True(result.StandardOutput.Length > 50, "Should have substantial output from the assistant");
    }
}
