using GitHub.Copilot.SDK;
using System.Reflection;

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
    public static SessionEvent CreateSessionIdleEvent()
    {
        var dataType = typeof(SessionIdleEvent).GetProperty("Data")?.PropertyType 
            ?? throw new InvalidOperationException("SessionIdleEvent does not have Data property");
        var data = Activator.CreateInstance(dataType) 
            ?? throw new InvalidOperationException($"Could not create instance of {dataType.Name}");
        
        return CreateEventWithData<SessionIdleEvent>(data);
    }

    /// <summary>
    /// <see cref="AssistantMessageEvent"/> を作成します
    /// </summary>
    /// <param name="content">メッセージ内容</param>
    public static SessionEvent CreateAssistantMessageEvent(string content)
    {
        var dataType = typeof(AssistantMessageEvent).GetProperty("Data")?.PropertyType 
            ?? throw new InvalidOperationException("AssistantMessageEvent does not have Data property");
        var data = Activator.CreateInstance(dataType) 
            ?? throw new InvalidOperationException($"Could not create instance of {dataType.Name}");
        
        SetProperty(data, "MessageId", Guid.NewGuid().ToString());
        SetProperty(data, "Content", content);
        
        return CreateEventWithData<AssistantMessageEvent>(data);
    }

    /// <summary>
    /// <see cref="AssistantMessageDeltaEvent"/> を作成します
    /// </summary>
    /// <param name="deltaContent">デルタ内容</param>
    public static SessionEvent CreateAssistantMessageDeltaEvent(string deltaContent)
    {
        var dataType = typeof(AssistantMessageDeltaEvent).GetProperty("Data")?.PropertyType 
            ?? throw new InvalidOperationException("AssistantMessageDeltaEvent does not have Data property");
        var data = Activator.CreateInstance(dataType) 
            ?? throw new InvalidOperationException($"Could not create instance of {dataType.Name}");
        
        SetProperty(data, "MessageId", Guid.NewGuid().ToString());
        SetProperty(data, "DeltaContent", deltaContent);
        
        return CreateEventWithData<AssistantMessageDeltaEvent>(data);
    }

    /// <summary>
    /// <see cref="AssistantReasoningEvent"/> を作成します
    /// </summary>
    /// <param name="content">推論内容</param>
    public static SessionEvent CreateAssistantReasoningEvent(string content)
    {
        var dataType = typeof(AssistantReasoningEvent).GetProperty("Data")?.PropertyType 
            ?? throw new InvalidOperationException("AssistantReasoningEvent does not have Data property");
        var data = Activator.CreateInstance(dataType) 
            ?? throw new InvalidOperationException($"Could not create instance of {dataType.Name}");
        
        SetProperty(data, "ReasoningId", Guid.NewGuid().ToString());
        SetProperty(data, "Content", content);
        
        return CreateEventWithData<AssistantReasoningEvent>(data);
    }

    /// <summary>
    /// <see cref="AssistantReasoningDeltaEvent"/> を作成します
    /// </summary>
    /// <param name="deltaContent">デルタ内容</param>
    public static SessionEvent CreateAssistantReasoningDeltaEvent(string deltaContent)
    {
        var dataType = typeof(AssistantReasoningDeltaEvent).GetProperty("Data")?.PropertyType 
            ?? throw new InvalidOperationException("AssistantReasoningDeltaEvent does not have Data property");
        var data = Activator.CreateInstance(dataType) 
            ?? throw new InvalidOperationException($"Could not create instance of {dataType.Name}");
        
        SetProperty(data, "ReasoningId", Guid.NewGuid().ToString());
        SetProperty(data, "DeltaContent", deltaContent);
        
        return CreateEventWithData<AssistantReasoningDeltaEvent>(data);
    }

    private static SessionEvent CreateEventWithData<TEvent>(object data) where TEvent : SessionEvent
    {
        var eventType = typeof(TEvent);
        var evt = Activator.CreateInstance(eventType) 
            ?? throw new InvalidOperationException($"Could not create instance of {eventType.Name}");
        
        var dataProperty = eventType.GetProperty("Data") 
            ?? throw new InvalidOperationException($"Event type {eventType.Name} does not have Data property");
        dataProperty.SetValue(evt, data);
        
        return (SessionEvent)evt;
    }

    private static void SetProperty(object obj, string propertyName, object? value)
    {
        var property = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance) 
            ?? throw new InvalidOperationException($"Type {obj.GetType().Name} does not have property {propertyName}");
        property.SetValue(obj, value);
    }
}
