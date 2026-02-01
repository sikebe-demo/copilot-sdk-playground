using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Abstractions;

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
