using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// <see cref="CopilotClient"/> の生成を抽象化するファクトリインターフェース
/// </summary>
public interface ICopilotClientFactory
{
    /// <summary>
    /// CopilotClient のインスタンスを生成します
    /// </summary>
    /// <param name="options">クライアントオプション</param>
    /// <returns>CopilotClient インスタンス</returns>
    CopilotClient Create(CopilotClientOptions options);
}
