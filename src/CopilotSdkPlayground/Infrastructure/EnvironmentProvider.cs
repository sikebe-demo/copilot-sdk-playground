using CopilotSdkPlayground.Abstractions;

namespace CopilotSdkPlayground.Infrastructure;

/// <summary>
/// <see cref="IEnvironmentProvider"/> の本番実装
/// </summary>
public class EnvironmentProvider : IEnvironmentProvider
{
    /// <inheritdoc />
    public string? GetEnvironmentVariable(string variable) => Environment.GetEnvironmentVariable(variable);
}
