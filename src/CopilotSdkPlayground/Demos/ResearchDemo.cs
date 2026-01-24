using CopilotSdkPlayground.Abstractions;
using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.Demos;

/// <summary>
/// Research デモ
/// claude-agent-sdk-demos の research-agent デモを GitHub Copilot SDK に移植したサンプル
/// トピックを分析し、研究ノートを生成するマルチステップリサーチをデモします
/// </summary>
public class ResearchDemoService(IConsoleWriter consoleWriter) : IResearchDemo
{
    /// <summary>
    /// リサーチエージェントのシステムプロンプト
    /// </summary>
    public const string SystemPrompt = @"You are a research coordinator who helps users conduct comprehensive research on any topic.

**Your Role:**
- Break down research requests into 2-4 distinct subtopics
- Provide structured research findings for each subtopic
- Synthesize information into clear, organized reports
- Focus on quantitative data when available (statistics, percentages, growth rates)

**Output Format:**
When asked to research a topic, respond with:
1. **Topic Analysis**: Brief overview of what you'll research
2. **Subtopics**: 2-4 distinct angles to investigate
3. **Research Findings**: Key information for each subtopic with sources
4. **Summary**: A concise synthesis of all findings

Keep responses focused, data-driven, and well-organized.";

    private readonly IConsoleWriter _consoleWriter = consoleWriter ?? throw new ArgumentNullException(nameof(consoleWriter));

    /// <inheritdoc />
    public async Task RunAsync(ICopilotClientWrapper client)
    {
        _consoleWriter.WriteLine("=== Research Agent Demo ===");
        _consoleWriter.WriteLine();
        _consoleWriter.WriteLine("This demo shows a research coordinator that can help you");
        _consoleWriter.WriteLine("research any topic by breaking it into subtopics and");
        _consoleWriter.WriteLine("providing structured findings.");
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
                    // 最終メッセージ完了時に改行
                    _consoleWriter.WriteLine();
                    break;

                case SessionIdleEvent:
                    // セッション完了
                    done.SetResult();
                    break;
            }
        });

        // Include system prompt context in the initial message
        var initialPrompt = $@"{SystemPrompt}

Please introduce yourself briefly and explain what kind of research topics you can help with.";

        _consoleWriter.Write("Research Agent: ");
        await session.SendAsync(new MessageOptions
        {
            Prompt = initialPrompt
        });
        await done.Task;
    }
}
