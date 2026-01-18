using GitHub.Copilot.SDK;

namespace CopilotSdkPlayground.UnitTests.TestHelpers;

/// <summary>
/// テスト用の SDK イベントファクトリ
/// SDK の型をリフレクションで作成します
/// </summary>
public static class EventFactory
{
    /// <summary>
    /// SessionIdleEvent を作成します
    /// </summary>
    public static object CreateSessionIdleEvent() => new SessionIdleEvent();

    /// <summary>
    /// <see cref="AssistantMessageEvent"/> を作成します
    /// </summary>
    /// <param name="content">メッセージ内容</param>
    public static object CreateAssistantMessageEvent(string content)
    {
        var evt = new AssistantMessageEvent();
        SetDataContent(evt, content, "Content");
        return evt;
    }

    /// <summary>
    /// <see cref="AssistantMessageDeltaEvent"/> を作成します
    /// </summary>
    /// <param name="deltaContent">デルタ内容</param>
    public static object CreateAssistantMessageDeltaEvent(string deltaContent)
    {
        var evt = new AssistantMessageDeltaEvent();
        SetDataContent(evt, deltaContent, "DeltaContent");
        return evt;
    }

    /// <summary>
    /// <see cref="AssistantReasoningEvent"/> を作成します
    /// </summary>
    /// <param name="content">推論内容</param>
    public static object CreateAssistantReasoningEvent(string content)
    {
        var evt = new AssistantReasoningEvent();
        SetDataContent(evt, content, "Content");
        return evt;
    }

    /// <summary>
    /// <see cref="AssistantReasoningDeltaEvent"/> を作成します
    /// </summary>
    /// <param name="deltaContent">デルタ内容</param>
    public static object CreateAssistantReasoningDeltaEvent(string deltaContent)
    {
        var evt = new AssistantReasoningDeltaEvent();
        SetDataContent(evt, deltaContent, "DeltaContent");
        return evt;
    }

    private static void SetDataContent(object evt, string content, string propertyName)
    {
        var dataProperty = evt.GetType().GetProperty("Data") ?? throw new InvalidOperationException($"Event type {evt.GetType().Name} does not have Data property");
        var dataType = dataProperty.PropertyType;
        var dataInstance = Activator.CreateInstance(dataType) ?? throw new InvalidOperationException($"Could not create instance of {dataType.Name}");
        var contentProperty = dataType.GetProperty(propertyName) ?? throw new InvalidOperationException($"Data type {dataType.Name} does not have {propertyName} property");
        contentProperty.SetValue(dataInstance, content);
        dataProperty.SetValue(evt, dataInstance);
    }
}
