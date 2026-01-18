using CopilotSdkPlayground.Abstractions;
using CopilotSdkPlayground.Infrastructure;
using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;

namespace CopilotSdkPlayground;

/// <summary>
/// アプリケーションのメインエントリポイントクラス
/// </summary>
public class App(
    ICopilotClientFactory clientFactory,
    IStreamingDemo streamingDemo,
    INonStreamingDemo nonStreamingDemo,
    ICopilotClientInfoLogger clientInfoLogger,
    ILogger<App> logger,
    ILogger<CopilotClient> copilotLogger)
{
    private readonly ICopilotClientFactory _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    private readonly IStreamingDemo _streamingDemo = streamingDemo ?? throw new ArgumentNullException(nameof(streamingDemo));
    private readonly INonStreamingDemo _nonStreamingDemo = nonStreamingDemo ?? throw new ArgumentNullException(nameof(nonStreamingDemo));
    private readonly ICopilotClientInfoLogger _clientInfoLogger = clientInfoLogger ?? throw new ArgumentNullException(nameof(clientInfoLogger));
    private readonly ILogger<App> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ILogger<CopilotClient> _copilotLogger = copilotLogger ?? throw new ArgumentNullException(nameof(copilotLogger));

    /// <summary>
    /// アプリケーションを実行します
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    /// <returns>終了コード</returns>
    public async Task<int> RunAsync(string[] args)
    {
        await using var client = _clientFactory.Create(new CopilotClientOptions
        {
            LogLevel = "debug",
            Logger = _copilotLogger,
        });

        var clientWrapper = new CopilotClientWrapper(client);

        try
        {
            if (IsNoStreamingMode(args))
            {
                await _nonStreamingDemo.RunAsync(clientWrapper);
            }
            else
            {
                await _streamingDemo.RunAsync(clientWrapper);
            }

            _clientInfoLogger.LogConnectionInfo(client);
            return 0;
        }
        catch (StreamJsonRpc.RemoteInvocationException ex)
        {
            _logger.LogError("JSON-RPC Error: {Message}", ex.Message);
            return 1;
        }
        catch (TimeoutException ex)
        {
            _logger.LogError("Timeout: {Message}", ex.Message);
            return 1;
        }
        catch (Exception ex)
        {
            _logger.LogError("Error: {Type} - {Message}", ex.GetType().Name, ex.Message);
            return 1;
        }
    }

    /// <summary>
    /// 非ストリーミングモードかどうかを判定します
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    /// <returns>非ストリーミングモードの場合は true</returns>
    public static bool IsNoStreamingMode(string[] args)
    {
        return args.Any(arg => arg.Equals("--no-streaming", StringComparison.OrdinalIgnoreCase));
    }
}
