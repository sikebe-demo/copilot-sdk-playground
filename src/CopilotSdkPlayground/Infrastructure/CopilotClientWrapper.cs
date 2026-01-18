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
    private readonly object _session;
    private readonly Action<Action<object>> _onMethod;
    private readonly Func<MessageOptions, Task> _sendAsyncMethod;

    public CopilotSessionWrapper(object session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));

        var sessionType = session.GetType();

        // Onメソッドを取得
        var onMethod = sessionType.GetMethod("On") ?? throw new InvalidOperationException("Session does not have On method");
        _onMethod = handler => onMethod.Invoke(_session, [handler]);

        // SendAsyncメソッドを取得
        var sendMethod = sessionType.GetMethod("SendAsync") ?? throw new InvalidOperationException("Session does not have SendAsync method");
        _sendAsyncMethod = options => (Task)sendMethod.Invoke(_session, [options])!;
    }

    /// <inheritdoc />
    public void On(Action<object> handler) => _onMethod(handler);

    /// <inheritdoc />
    public Task SendAsync(MessageOptions options) => _sendAsyncMethod(options);
}
