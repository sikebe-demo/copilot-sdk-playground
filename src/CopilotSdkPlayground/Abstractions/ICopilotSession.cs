using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Abstractions;

/// <summary>
///<see cref="CopilotSession"/> のインターフェース
/// </summary>
public interface ICopilotSession
{
    /// <summary>
    /// イベントハンドラを登録します
    /// </summary>
    /// <param name="handler">イベントハンドラ</param>
    void On(Action<object> handler);

    /// <summary>
    /// メッセージを送信します
    /// </summary>
    /// <param name="options">メッセージオプション</param>
    /// <returns>非同期タスク</returns>
    Task SendAsync(MessageOptions options);
}

/// <summary>
/// Copilot クライアントのインターフェース
/// </summary>
public interface ICopilotClientWrapper
{
    /// <summary>
    /// セッションを作成します
    /// </summary>
    /// <param name="config">セッション設定</param>
    /// <returns>Copilot セッション</returns>
    Task<ICopilotSession> CreateSessionAsync(SessionConfig config);
}
