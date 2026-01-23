using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Abstractions;

/// <summary>
/// <see cref="CopilotClient"/> 情報のロギングを抽象化するインターフェース
/// </summary>
public interface ICopilotClientInfoLogger
{
    /// <summary>
    /// クライアント接続情報をログに出力します
    /// </summary>
    /// <param name="client">CopilotClient インスタンス</param>
    /// <param name="options">CopilotClientOptions インスタンス</param>
    Task LogConnectionInfoAsync(CopilotClient client, CopilotClientOptions options);
}
