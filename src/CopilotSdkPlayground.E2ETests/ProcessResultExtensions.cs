namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// ProcessResult の拡張メソッド
/// </summary>
public static class ProcessResultExtensions
{
    /// <summary>
    /// プロセス実行結果をテスト出力に記録します
    /// </summary>
    public static void LogTo(this ProcessResult result, ITestOutputHelper output)
    {
        output.WriteLine("========== Process Result ==========");
        output.WriteLine($"Exit Code: {result.ExitCode}");
        output.WriteLine($"Timed Out: {result.TimedOut}");
        output.WriteLine("");
        output.WriteLine("---------- Standard Output ----------");
        output.WriteLine(result.StandardOutput);
        if (!string.IsNullOrWhiteSpace(result.StandardError))
        {
            output.WriteLine("");
            output.WriteLine("---------- Standard Error ----------");
            output.WriteLine(result.StandardError);
        }
        output.WriteLine("=====================================");
    }
}
