namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// ストリーミングモードの E2E テスト
/// ビルド済みアプリケーションをプロセスとして実行します
/// </summary>
[Collection("Process")]
public class StreamingE2ETests(ProcessFixture fixture, ITestOutputHelper output)
{
    private readonly ProcessFixture _fixture = fixture;
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task StreamingMode_ShouldRunSuccessfullyWithExpectedOutput()
    {
        // Skip if not available
        if (!_fixture.IsAvailable)
        {
            Assert.Skip(_fixture.SkipReason ?? "Test is not available.");
        }

        // Arrange
        var args = Array.Empty<string>(); // デフォルトはストリーミングモード

        // Act
        var result = await _fixture.RunApplicationAsync(args);
        result.LogTo(_output);

        // Assert - 正常終了
        Assert.False(result.TimedOut, $"Process should not timeout. StdErr: {result.StandardError}");
        Assert.Equal(0, result.ExitCode);

        // Assert - 期待される出力
        Assert.Contains("=== Streaming Mode Demo ===", result.StandardOutput);
        Assert.Contains("Assistant:", result.StandardOutput);
        Assert.Contains("--- Final message ---", result.StandardOutput);
        Assert.True(result.StandardOutput.Length > 100, "Should have substantial output from the assistant");
    }
}
