using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Demos;

/// <summary>
/// 非ストリーミングモードのデモ
/// 完了したレスポンスをまとめて受信して表示します
/// </summary>
public static class NonStreamingDemo
{
    public static async Task RunAsync(CopilotClient client)
    {
        Console.WriteLine("=== Non-Streaming Mode Demo ===");
        Console.WriteLine();

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
                    Console.WriteLine(msg.Data.Content);
                    break;

                case AssistantReasoningEvent reasoningEvt:
                    // 推論結果を表示
                    Console.WriteLine("--- Reasoning ---");
                    Console.WriteLine(reasoningEvt.Data.Content);
                    break;

                case SessionIdleEvent:
                    // セッション完了
                    done.SetResult();
                    break;
            }
        });

        Console.WriteLine("Assistant: ");
        await session.SendAsync(new MessageOptions { Prompt = "小話をして" });
        await done.Task;
    }
}
