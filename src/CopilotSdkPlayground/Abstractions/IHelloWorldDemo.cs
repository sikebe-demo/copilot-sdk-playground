namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// Hello World デモのインターフェース
/// claude-agent-sdk-demos の hello-world デモを GitHub Copilot SDK に移植したサンプル
/// </summary>
public interface IHelloWorldDemo
{
    /// <summary>
    /// Hello World デモを実行します
    /// </summary>
    /// <param name="client">CopilotClient ラッパー インスタンス</param>
    /// <returns>非同期タスク</returns>
    Task RunAsync(ICopilotClientWrapper client);
}
