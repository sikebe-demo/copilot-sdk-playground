using CopilotSdkPlayground.Abstractions;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Demos;

/// <summary>
/// ストリーミングモードのデモ
/// レスポンスをリアルタイムで受信して表示します
/// </summary>
public class StreamingDemoService(IConsoleWriter consoleWriter) : IStreamingDemo
{
    private readonly IConsoleWriter _consoleWriter = consoleWriter ?? throw new ArgumentNullException(nameof(consoleWriter));

    /// <inheritdoc />
    public async Task RunAsync(ICopilotClientWrapper client)
    {
        _consoleWriter.WriteLine("=== Streaming Mode Demo ===");
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

                case AssistantReasoningDeltaEvent reasoningDelta:
                    // 推論中のテキスト断片を表示
                    _consoleWriter.Write(reasoningDelta.Data.DeltaContent);
                    break;

                case AssistantMessageEvent msg:
                    // 最終メッセージを表示
                    _consoleWriter.WriteLine();
                    _consoleWriter.WriteLine("--- Final message ---");
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

        // タイムアウト付きで完了を待機（無限待機を防ぐ）
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
        try
        {
            await done.Task.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Session did not complete within the timeout period.");
        }
    }
}
