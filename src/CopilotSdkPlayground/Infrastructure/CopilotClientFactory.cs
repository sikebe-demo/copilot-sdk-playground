using CopilotSdkPlayground.Abstractions;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Infrastructure;

/// <summary>
/// <see cref="ICopilotClientFactory"/> の本番実装
/// </summary>
public class CopilotClientFactory : ICopilotClientFactory
{
    /// <inheritdoc />
    public CopilotClient Create(CopilotClientOptions options) => new(options);
}
