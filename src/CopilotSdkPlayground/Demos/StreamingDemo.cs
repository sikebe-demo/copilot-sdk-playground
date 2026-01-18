using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Demos;

/// <summary>
/// ストリーミングモードのデモ
/// レスポンスをリアルタイムで受信して表示します
/// </summary>
public static class StreamingDemo
{
    public static async Task RunAsync(CopilotClient client)
    {
        Console.WriteLine("=== Streaming Mode Demo ===");
        Console.WriteLine();

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
                    Console.Write(delta.Data.DeltaContent);
                    break;

                case AssistantReasoningDeltaEvent reasoningDelta:
                    // 推論中のテキスト断片を表示
                    Console.Write(reasoningDelta.Data.DeltaContent);
                    break;

                case AssistantMessageEvent msg:
                    // 最終メッセージを表示
                    Console.WriteLine();
                    Console.WriteLine("--- Final message ---");
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
