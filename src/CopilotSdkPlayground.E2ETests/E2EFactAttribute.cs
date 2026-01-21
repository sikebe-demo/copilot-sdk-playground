using Xunit;

namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// Fact attribute that skips when E2E prerequisites like tokens or build output are missing.
/// </summary>
public sealed class E2EFactAttribute : FactAttribute
{
    public E2EFactAttribute()
    {
        Skip = ProcessFixture.GetSkipReason();
    }
}
