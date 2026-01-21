using Xunit;

namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// E2E テスト用の Fact 属性
/// </summary>
public sealed class E2EFactAttribute : FactAttribute
{
    public E2EFactAttribute()
    {
        Skip = ProcessFixture.GetSkipReason();
    }
}
