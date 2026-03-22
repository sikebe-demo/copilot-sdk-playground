namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// Research デモの E2E テスト
/// ビルド済みアプリケーションをプロセスとして実行します
/// </summary>
[Collection("Process")]
public class ResearchE2ETests(ProcessFixture fixture, ITestOutputHelper output)
{
    private readonly ProcessFixture _fixture = fixture;
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task ResearchMode_ShouldRunSuccessfullyWithExpectedOutput()
    {
        // Skip if not available
        if (!_fixture.IsAvailable)
        {
            Assert.Skip(_fixture.SkipReason ?? "Test is not available.");
        }

        // Arrange
        var args = new[] { "--research" };

        // Act
        var result = await _fixture.RunApplicationAsync(args);
        result.LogTo(_output);

        // Assert - 正常終了
        Assert.False(result.TimedOut, $"Process should not timeout. StdErr: {result.StandardError}");
        Assert.Equal(0, result.ExitCode);

        // Assert - 期待される出力
        Assert.Contains("=== Research Agent Demo ===", result.StandardOutput);
        Assert.Contains("Research Agent:", result.StandardOutput);
        Assert.True(result.StandardOutput.Length > 100, "Should have substantial output from the research agent");
    }
}
