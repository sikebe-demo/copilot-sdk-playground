namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// ProcessResult の拡張メソッド
/// </summary>
public static class ProcessResultExtensions
{
    /// <summary>
    /// プロセス実行結果をテスト出力に記録します。
    /// GitHub Actions のログに出力するため、Console.WriteLine も使用します。
    /// </summary>
    public static void LogTo(this ProcessResult result, ITestOutputHelper output)
    {
        var lines = new[]
        {
            "========== Process Result ==========",
            $"Exit Code: {result.ExitCode}",
            $"Timed Out: {result.TimedOut}",
            "",
            "---------- Standard Output ----------",
            result.StandardOutput
        };

        foreach (var line in lines)
        {
            output.WriteLine(line);
            Console.WriteLine(line);
        }

        if (!string.IsNullOrWhiteSpace(result.StandardError))
        {
            var errorLines = new[]
            {
                "",
                "---------- Standard Error ----------",
                result.StandardError
            };

            foreach (var line in errorLines)
            {
                output.WriteLine(line);
                Console.WriteLine(line);
            }
        }

        output.WriteLine("=====================================");
        Console.WriteLine("=====================================");
    }
}
