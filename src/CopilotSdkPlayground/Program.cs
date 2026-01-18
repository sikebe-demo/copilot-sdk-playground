using CopilotSdkPlayground;
using CopilotSdkPlayground.Demos;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;

var useStreaming = !(args.Length > 0 && args[0].Equals("--no-streaming", StringComparison.OrdinalIgnoreCase));

using var loggerFactory = LoggerSetup.CreateLoggerFactory();
var logger = loggerFactory.CreateLogger<CopilotClient>();

await using var client = new CopilotClient(new CopilotClientOptions
{
    LogLevel = "debug",
    Logger = logger,
});

try
{
    if (useStreaming)
    {
        await StreamingDemo.RunAsync(client);
    }
    else
    {
        await NonStreamingDemo.RunAsync(client);
    }

    CopilotClientInfoLogger.LogConnectionInfo(client, logger);
}
catch (StreamJsonRpc.RemoteInvocationException ex)
{
    logger.LogError("JSON-RPC Error: {Message}", ex.Message);
}
catch (TimeoutException ex)
{
    logger.LogError("Timeout: {Message}", ex.Message);
}
catch (Exception ex)
{
    logger.LogError("Error: {Type} - {Message}", ex.GetType().Name, ex.Message);
}
