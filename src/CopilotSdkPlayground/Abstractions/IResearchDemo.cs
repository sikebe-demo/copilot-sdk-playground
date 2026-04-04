namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// Research デモのインターフェース
/// claude-agent-sdk-demos の research-agent デモを GitHub Copilot SDK に移植したサンプル
/// </summary>
public interface IResearchDemo
{
    /// <summary>
    /// Research デモを実行します
    /// </summary>
    /// <param name="client">CopilotClient ラッパー インスタンス</param>
    /// <returns>非同期タスク</returns>
    Task RunAsync(ICopilotClientWrapper client);
}
