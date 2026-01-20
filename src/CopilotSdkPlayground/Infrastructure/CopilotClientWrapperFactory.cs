using CopilotSdkPlayground.Abstractions;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Infrastructure;

/// <summary>
/// <see cref="ICopilotClientWrapperFactory"/> の実装
/// </summary>
public class CopilotClientWrapperFactory : ICopilotClientWrapperFactory
{
    /// <inheritdoc />
    public ICopilotClientWrapper Create(CopilotClient client)
    {
        return new CopilotClientWrapper(client);
    }
}
