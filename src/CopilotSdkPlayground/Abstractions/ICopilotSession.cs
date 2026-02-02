using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// <see cref="CopilotSession"/> のインターフェース
/// </summary>
public interface ICopilotSession
{
    /// <summary>
    /// イベントハンドラを登録します
    /// </summary>
    /// <param name="handler">イベントハンドラ</param>
    void On(SessionEventHandler handler);

    /// <summary>
    /// メッセージを送信します
    /// </summary>
    /// <param name="options">メッセージオプション</param>
    /// <returns>非同期タスク</returns>
    Task SendAsync(MessageOptions options);
}
