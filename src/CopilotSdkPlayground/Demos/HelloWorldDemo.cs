using CopilotSdkPlayground.Abstractions;
using CopilotSdkPlayground.Helpers;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Demos;

/// <summary>
/// Hello World デモ
/// claude-agent-sdk-demos の hello-world デモを GitHub Copilot SDK に移植したサンプル
/// AI に自己紹介を依頼するシンプルなデモです
/// </summary>
public class HelloWorldDemoService(IConsoleWriter consoleWriter) : IHelloWorldDemo
{
    private readonly IConsoleWriter _consoleWriter = consoleWriter ?? throw new ArgumentNullException(nameof(consoleWriter));

    /// <inheritdoc />
    public async Task RunAsync(ICopilotClientWrapper client)
    {
        _consoleWriter.WriteLine("=== Hello World Demo ===");
        _consoleWriter.WriteLine();

        var session = await client.CreateSessionAsync(new SessionConfig
        {
            Model = "gpt-5",
            Streaming = true
        });

        var done = new TaskCompletionSource();

        session.On(evt =>
        {
            switch (evt)
            {
                case AssistantMessageDeltaEvent delta:
                    // ストリーミング中のテキスト断片を表示
                    _consoleWriter.Write(delta.Data.DeltaContent);
                    break;

                case AssistantMessageEvent:
                    // 最終メッセージを表示
                    _consoleWriter.WriteLine();
                    break;

                case SessionIdleEvent:
                    // セッション完了
                    done.SetResult();
                    break;
            }
        });

        _consoleWriter.Write("Copilot says: ");
        await session.SendAsync(new MessageOptions { Prompt = "Hello, Copilot! Please introduce yourself in one sentence." });

        await done.WaitWithTimeoutAsync(TimeSpan.FromMinutes(2));
    }
}
