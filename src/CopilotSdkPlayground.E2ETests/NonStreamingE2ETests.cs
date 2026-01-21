namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// 非ストリーミングモードの E2E テスト
/// ビルド済みアプリケーションをプロセスとして実行します
/// </summary>
[Collection("Process")]
public class NonStreamingE2ETests(ProcessFixture fixture, ITestOutputHelper output)
{
    private readonly ProcessFixture _fixture = fixture;
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task NonStreamingMode_ShouldRunSuccessfullyWithExpectedOutput()
    {
        // Skip if not available
        if (!_fixture.IsAvailable)
        {
            Assert.Skip(_fixture.SkipReason ?? "Test is not available.");
        }

        // Arrange
        var args = new[] { "--no-streaming" };

        // Act
        var result = await _fixture.RunApplicationAsync(args);
        result.LogTo(_output);

        // Assert - 正常終了
        Assert.False(result.TimedOut, $"Process should not timeout. StdErr: {result.StandardError}");
        Assert.Equal(0, result.ExitCode);

        // Assert - 期待される出力
        Assert.Contains("=== Non-Streaming Mode Demo ===", result.StandardOutput);
        Assert.Contains("Assistant:", result.StandardOutput);
        Assert.True(result.StandardOutput.Length > 100, "Should have substantial output from the assistant");

        // Assert - 完全なメッセージが出力されていること
        var lines = result.StandardOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var contentLines = lines.Where(l =>
            !l.Contains("===") &&
            !l.Contains("Assistant:") &&
            !l.Contains("---") &&
            !l.Contains(':') &&
            !string.IsNullOrWhiteSpace(l)).ToList();
        Assert.NotEmpty(contentLines);
    }
}
