using System.Diagnostics;
using System.Reflection;
using CopilotSdkPlayground.Abstractions;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;

namespace CopilotSdkPlayground;

/// <summary>
/// Copilot クライアント情報のロギングサービス
/// </summary>
public class CopilotClientInfoLoggerService(
    ILogger<CopilotClientInfoLoggerService> logger,
    IEnvironmentProvider environmentProvider,
    IFileSystem fileSystem) : ICopilotClientInfoLogger
{
    private const BindingFlags _privateInstance = BindingFlags.NonPublic | BindingFlags.Instance;

    private readonly ILogger<CopilotClientInfoLoggerService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IEnvironmentProvider _environmentProvider = environmentProvider ?? throw new ArgumentNullException(nameof(environmentProvider));
    private readonly IFileSystem _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));

    /// <inheritdoc />
    public void LogConnectionInfo(CopilotClient client)
    {
        _logger.LogInformation("=== Copilot Client Connection Info ===");

        LogOptionsInfo(client);
        LogProcessInfo(client);

        _logger.LogInformation("=======================================");
    }

    private void LogOptionsInfo(CopilotClient client)
    {
        var options = GetFieldValue<object>(client, "_options");
        if (options == null) return;

        var cliPath = GetPropertyValue<string>(options, "CliPath");
        var port = GetPropertyValue<int>(options, "Port");
        var useStdio = GetPropertyValue<bool>(options, "UseStdio");

        _logger.LogInformation("  Configured CLI Path: {CliPath}", cliPath ?? "(auto-detected)");
        _logger.LogInformation("  Configured Port: {Port} (UseStdio: {UseStdio})", port, useStdio);
    }

    private void LogProcessInfo(CopilotClient client)
    {
        var connectionTask = GetFieldValue<object>(client, "_connectionTask");
        if (connectionTask == null) return;

        var connection = GetPropertyValue<object>(connectionTask, "Result");
        if (connection == null) return;

        var process = GetFieldValueByType<Process>(connection);
        if (process == null) return;

        _logger.LogInformation("  CLI Process ID: {PID}", process.Id);
        _logger.LogInformation("  CLI Command: {FileName}", process.StartInfo.FileName);

        var arguments = process.StartInfo.Arguments;
        if (string.IsNullOrEmpty(arguments)) return;

        _logger.LogInformation("  CLI Arguments: {Arguments}", arguments);

        // 引数から実際のCopilot CLIを抽出（例: /c copilot --server ...）
        if (arguments.StartsWith("/c ", StringComparison.OrdinalIgnoreCase))
        {
            var cliCommand = arguments[3..].Split(' ')[0];
            var resolvedPath = ResolveCopilotCliPath(cliCommand);
            if (resolvedPath != null)
            {
                _logger.LogInformation("  Resolved CLI Path: {ResolvedPath}", resolvedPath);
            }
        }
    }

    private string? ResolveCopilotCliPath(string cliCommand)
    {
        var pathEnv = _environmentProvider.GetEnvironmentVariable("PATH");
        if (pathEnv == null) return null;

        var extensions = new[] { ".cmd", ".bat", ".exe", "" };

        foreach (var path in pathEnv.Split(_fileSystem.PathSeparator))
        {
            foreach (var ext in extensions)
            {
                var fullPath = _fileSystem.CombinePath(path, cliCommand + ext);
                if (_fileSystem.FileExists(fullPath))
                {
                    return fullPath;
                }
            }
        }

        return null;
    }

    private static T? GetFieldValue<T>(object obj, string fieldName) where T : class
    {
        try
        {
            var field = obj.GetType().GetField(fieldName, _privateInstance);
            return field?.GetValue(obj) as T;
        }
        catch
        {
            return null;
        }
    }

    private static T? GetPropertyValue<T>(object obj, string propertyName)
    {
        try
        {
            var prop = obj.GetType().GetProperty(propertyName);
            var value = prop?.GetValue(obj);
            return value is T typed ? typed : default;
        }
        catch
        {
            return default;
        }
    }

    private static T? GetFieldValueByType<T>(object obj) where T : class
    {
        try
        {
            foreach (var field in obj.GetType().GetFields(_privateInstance))
            {
                if (field.GetValue(obj) is T value)
                {
                    return value;
                }
            }
        }
        catch { }

        return null;
    }
}
