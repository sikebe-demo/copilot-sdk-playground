using CopilotSdkPlayground.Abstractions;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Infrastructure;

/// <summary>
/// <see cref="CopilotClient"/> のラッパー実装
/// </summary>
public class CopilotClientWrapper(CopilotClient client) : ICopilotClientWrapper
{
    private readonly CopilotClient _client = client ?? throw new ArgumentNullException(nameof(client));

    /// <inheritdoc />
    public async Task<ICopilotSession> CreateSessionAsync(SessionConfig config)
    {
        var session = await _client.CreateSessionAsync(config);
        return new CopilotSessionWrapper(session);
    }
}

/// <summary>
/// Session のラッパー実装
/// </summary>
public class CopilotSessionWrapper : ICopilotSession
{
    private readonly CopilotSession _session;

    public CopilotSessionWrapper(object session)
    {
        _session = session as CopilotSession ?? throw new ArgumentException("Session must be a CopilotSession", nameof(session));
    }

    /// <inheritdoc />
    public void On(SessionEventHandler handler) => _session.On(handler);

    /// <inheritdoc />
    public Task SendAsync(MessageOptions options) => _session.SendAsync(options);
}
