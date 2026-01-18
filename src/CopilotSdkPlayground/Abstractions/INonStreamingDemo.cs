namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// 非ストリーミングモードデモのインターフェース
/// </summary>
public interface INonStreamingDemo
{
    /// <summary>
    /// 非ストリーミングモードのデモを実行します
    /// </summary>
    /// <param name="client">CopilotClient ラッパー インスタンス</param>
    /// <returns>非同期タスク</returns>
    Task RunAsync(ICopilotClientWrapper client);
}
