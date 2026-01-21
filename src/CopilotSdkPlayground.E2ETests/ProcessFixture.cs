using System.Diagnostics;
using System.Text;

namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// アプリケーションをプロセスとして実行する E2E テスト用フィクスチャ
/// ビルド済みの exe/dll を実際に起動して検証します
/// </summary>
public class ProcessFixture : IAsyncLifetime
{
    public bool IsAvailable { get; private set; }
    public string? SkipReason { get; private set; }
    public string ApplicationPath { get; private set; } = string.Empty;

    public ValueTask InitializeAsync()
    {
        // GH_TOKEN or GITHUB_TOKEN is used by Copilot CLI for authentication
        var authToken = Environment.GetEnvironmentVariable("GH_TOKEN")
            ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");

        if (string.IsNullOrEmpty(authToken))
        {
            SkipReason = "GH_TOKEN or GITHUB_TOKEN environment variable is not set. E2E tests require authentication.";
            IsAvailable = false;
            return ValueTask.CompletedTask;
        }

        // ビルド済みの dll パスを特定
        // テストプロジェクトの出力ディレクトリから相対的に本体プロジェクトの dll を参照
        var testAssemblyPath = typeof(ProcessFixture).Assembly.Location;
        var testOutputDir = Path.GetDirectoryName(testAssemblyPath)!;

        // src/CopilotSdkPlayground.E2ETests/bin/Debug/net10.0 から
        // src/CopilotSdkPlayground/bin/Debug/net10.0 への相対パス
        var appOutputDir = Path.GetFullPath(
            Path.Combine(testOutputDir, "..", "..", "..", "..", "CopilotSdkPlayground", "bin", "Debug", "net10.0"));
        ApplicationPath = Path.Combine(appOutputDir, "CopilotSdkPlayground.dll");

        if (!File.Exists(ApplicationPath))
        {
            SkipReason = $"Application not found at: {ApplicationPath}. Please build the solution first.";
            IsAvailable = false;
            return ValueTask.CompletedTask;
        }

        IsAvailable = true;
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    /// <summary>
    /// アプリケーションを実行し、結果を取得します
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    /// <param name="timeout">タイムアウト（デフォルト120秒）</param>
    /// <returns>実行結果</returns>
    public async Task<ProcessResult> RunApplicationAsync(string[] args, TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromSeconds(120);

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"\"{ApplicationPath}\" {string.Join(" ", args)}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            // 環境変数を引き継ぐ（GH_TOKEN など）
        };

        var stdOut = new StringBuilder();
        var stdErr = new StringBuilder();

        using var process = new Process { StartInfo = startInfo };
        using var cts = new CancellationTokenSource(timeout.Value);

        var outputTcs = new TaskCompletionSource();
        var errorTcs = new TaskCompletionSource();

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                stdOut.AppendLine(e.Data);
            }
            else
            {
                outputTcs.TrySetResult();
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                stdErr.AppendLine(e.Data);
            }
            else
            {
                errorTcs.TrySetResult();
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            await process.WaitForExitAsync(cts.Token);
            // 出力の読み取り完了を待つ
            await Task.WhenAll(outputTcs.Task, errorTcs.Task).WaitAsync(TimeSpan.FromSeconds(5));
        }
        catch (OperationCanceledException)
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch
            {
                // プロセスが既に終了している場合は無視
            }

            return new ProcessResult
            {
                ExitCode = -1,
                StandardOutput = stdOut.ToString(),
                StandardError = $"Process timed out after {timeout.Value.TotalSeconds} seconds.\n{stdErr}",
                TimedOut = true
            };
        }

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = stdOut.ToString(),
            StandardError = stdErr.ToString(),
            TimedOut = false
        };
    }
}

/// <summary>
/// プロセス実行結果
/// </summary>
public class ProcessResult
{
    /// <summary>
    /// 終了コード
    /// </summary>
    public int ExitCode { get; init; }

    /// <summary>
    /// 標準出力
    /// </summary>
    public string StandardOutput { get; init; } = string.Empty;

    /// <summary>
    /// 標準エラー出力
    /// </summary>
    public string StandardError { get; init; } = string.Empty;

    /// <summary>
    /// タイムアウトしたかどうか
    /// </summary>
    public bool TimedOut { get; init; }
}
