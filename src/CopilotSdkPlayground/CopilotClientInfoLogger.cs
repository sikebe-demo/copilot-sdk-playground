using System.Diagnostics;
using System.Reflection;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;

namespace CopilotSdkPlayground;

public static class CopilotClientInfoLogger
{
    private const BindingFlags _privateInstance = BindingFlags.NonPublic | BindingFlags.Instance;

    public static void LogConnectionInfo(CopilotClient client, ILogger logger)
    {
        logger.LogInformation("=== Copilot Client Connection Info ===");

        LogOptionsInfo(client, logger);
        LogProcessInfo(client, logger);

        logger.LogInformation("=======================================");
    }

    private static void LogOptionsInfo(CopilotClient client, ILogger logger)
    {
        var options = GetFieldValue<object>(client, "_options");
        if (options == null) return;

        var cliPath = GetPropertyValue<string>(options, "CliPath");
        var port = GetPropertyValue<int>(options, "Port");
        var useStdio = GetPropertyValue<bool>(options, "UseStdio");

        logger.LogInformation("  Configured CLI Path: {CliPath}", cliPath ?? "(auto-detected)");
        logger.LogInformation("  Configured Port: {Port} (UseStdio: {UseStdio})", port, useStdio);
    }

    private static void LogProcessInfo(CopilotClient client, ILogger logger)
    {
        var connectionTask = GetFieldValue<object>(client, "_connectionTask");
        if (connectionTask == null) return;

        var connection = GetPropertyValue<object>(connectionTask, "Result");
        if (connection == null) return;

        var process = GetFieldValueByType<Process>(connection);
        if (process == null) return;

        logger.LogInformation("  CLI Process ID: {PID}", process.Id);
        logger.LogInformation("  CLI Command: {FileName}", process.StartInfo.FileName);

        var arguments = process.StartInfo.Arguments;
        if (string.IsNullOrEmpty(arguments)) return;

        logger.LogInformation("  CLI Arguments: {Arguments}", arguments);

        // 引数から実際のCopilot CLIを抽出（例: /c copilot --server ...）
        if (arguments.StartsWith("/c ", StringComparison.OrdinalIgnoreCase))
        {
            var cliCommand = arguments[3..].Split(' ')[0];
            var resolvedPath = ResolveCopilotCliPath(cliCommand);
            if (resolvedPath != null)
            {
                logger.LogInformation("  Resolved CLI Path: {ResolvedPath}", resolvedPath);
            }
        }
    }

    private static string? ResolveCopilotCliPath(string cliCommand)
    {
        var pathEnv = Environment.GetEnvironmentVariable("PATH");
        if (pathEnv == null) return null;

        var extensions = new[] { ".cmd", ".bat", ".exe", "" };

        foreach (var path in pathEnv.Split(Path.PathSeparator))
        {
            foreach (var ext in extensions)
            {
                var fullPath = Path.Combine(path, cliCommand + ext);
                if (File.Exists(fullPath))
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
