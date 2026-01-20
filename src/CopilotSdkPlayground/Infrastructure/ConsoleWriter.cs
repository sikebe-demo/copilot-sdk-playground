using CopilotSdkPlayground.Abstractions;

namespace CopilotSdkPlayground.Infrastructure;

/// <summary>
/// <see cref="IConsoleWriter"/> の本番実装
/// </summary>
public class ConsoleWriter : IConsoleWriter
{
    /// <inheritdoc />
    public void Write(string? text) => Console.Write(text);

    /// <inheritdoc />
    public void WriteLine(string? text = null) => Console.WriteLine(text);
}
