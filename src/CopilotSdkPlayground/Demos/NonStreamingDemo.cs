using CopilotSdkPlayground.Abstractions;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Demos;

/// <summary>
/// 非ストリーミングモードのデモ
/// 完了したレスポンスをまとめて受信して表示します
/// </summary>
public class NonStreamingDemoService(IConsoleWriter consoleWriter) : INonStreamingDemo
{
    private readonly IConsoleWriter _consoleWriter = consoleWriter ?? throw new ArgumentNullException(nameof(consoleWriter));

    /// <inheritdoc />
    public async Task RunAsync(ICopilotClientWrapper client)
    {
        _consoleWriter.WriteLine("=== Non-Streaming Mode Demo ===");
        _consoleWriter.WriteLine();

        var session = await client.CreateSessionAsync(new SessionConfig
        {
            Model = "gpt-5",
            Streaming = false
        });

        var done = new TaskCompletionSource();

        session.On(evt =>
        {
            switch (evt)
            {
                case AssistantMessageEvent msg:
                    // 完了したメッセージを表示
                    _consoleWriter.WriteLine(msg.Data.Content);
                    break;

                case AssistantReasoningEvent reasoningEvt:
                    // 推論結果を表示
                    _consoleWriter.WriteLine("--- Reasoning ---");
                    _consoleWriter.WriteLine(reasoningEvt.Data.Content);
                    break;

                case SessionIdleEvent:
                    // セッション完了
                    done.SetResult();
                    break;
            }
        });

        _consoleWriter.WriteLine("Assistant: ");
        await session.SendAsync(new MessageOptions { Prompt = "小話をして" });
        await done.Task;
    }
}
