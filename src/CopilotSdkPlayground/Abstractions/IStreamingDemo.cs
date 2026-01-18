namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// ストリーミングモードデモのインターフェース
/// </summary>
public interface IStreamingDemo
{
    /// <summary>
    /// ストリーミングモードのデモを実行します
    /// </summary>
    /// <param name="client">CopilotClient ラッパー インスタンス</param>
    /// <returns>非同期タスク</returns>
    Task RunAsync(ICopilotClientWrapper client);
}
