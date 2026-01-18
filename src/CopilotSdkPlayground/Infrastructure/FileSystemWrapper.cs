using CopilotSdkPlayground.Abstractions;

namespace CopilotSdkPlayground.Infrastructure;

/// <summary>
/// <see cref="IFileSystem"/> の本番実装
/// </summary>
public class FileSystemWrapper : IFileSystem
{
    /// <inheritdoc />
    public bool FileExists(string path) => File.Exists(path);

    /// <inheritdoc />
    public string CombinePath(params string[] paths) => Path.Combine(paths);

    /// <inheritdoc />
    public char PathSeparator => Path.PathSeparator;
}
