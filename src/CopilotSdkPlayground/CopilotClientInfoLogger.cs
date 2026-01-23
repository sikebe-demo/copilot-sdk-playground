using CopilotSdkPlayground.Abstractions;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;

namespace CopilotSdkPlayground;

/// <summary>
/// Copilot クライアント情報のロギングサービス
/// </summary>
public class CopilotClientInfoLoggerService(
    ILogger<CopilotClientInfoLoggerService> logger) : ICopilotClientInfoLogger
{
    private readonly ILogger<CopilotClientInfoLoggerService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task LogConnectionInfoAsync(CopilotClient client, CopilotClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(options);

        _logger.LogInformation("=== Copilot Client Connection Info ===");

        LogOptionsInfo(options);
        await LogStatusInfoAsync(client);

        _logger.LogInformation("=======================================");
    }

    private void LogOptionsInfo(CopilotClientOptions options)
    {
        _logger.LogInformation("  Configured CLI Path: {CliPath}", options.CliPath ?? "(auto-detected)");
        _logger.LogInformation("  Configured Port: {Port} (UseStdio: {UseStdio})", options.Port, options.UseStdio);
    }

    private async Task LogStatusInfoAsync(CopilotClient client)
    {
        try
        {
            var status = await client.GetStatusAsync();
            _logger.LogInformation("  CLI Version: {Version}", status.Version);
            _logger.LogInformation("  Protocol Version: {ProtocolVersion}", status.ProtocolVersion);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("  Unable to retrieve CLI status: {Message}", ex.Message);
        }
    }
}
