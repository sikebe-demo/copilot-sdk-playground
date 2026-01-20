using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// CopilotClientWrapper を作成するファクトリのインターフェース
/// </summary>
public interface ICopilotClientWrapperFactory
{
    /// <summary>
    /// CopilotClient から CopilotClientWrapper を作成します
    /// </summary>
    /// <param name="client">Copilot クライアント</param>
    /// <returns>Copilot クライアントラッパー</returns>
    ICopilotClientWrapper Create(CopilotClient client);
}
