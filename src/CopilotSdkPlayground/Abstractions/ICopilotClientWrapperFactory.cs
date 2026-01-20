using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// <see cref="ICopilotClientWrapper"/> のファクトリインターフェース
/// </summary>
public interface ICopilotClientWrapperFactory
{
    /// <summary>
    /// <see cref="ICopilotClientWrapper"/> のインスタンスを作成します
    /// </summary>
    /// <param name="client">Copilot クライアント</param>
    /// <returns>Copilot クライアントラッパー</returns>
    ICopilotClientWrapper Create(CopilotClient client);
}
