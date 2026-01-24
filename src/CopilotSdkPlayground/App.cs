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
    IHelloWorldDemo helloWorldDemo,
    ICopilotClientInfoLogger clientInfoLogger,
    ILogger<App> logger,
    ILogger<CopilotClient> copilotLogger)
{
    private readonly ICopilotClientFactory _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
    private readonly IStreamingDemo _streamingDemo = streamingDemo ?? throw new ArgumentNullException(nameof(streamingDemo));
    private readonly INonStreamingDemo _nonStreamingDemo = nonStreamingDemo ?? throw new ArgumentNullException(nameof(nonStreamingDemo));
    private readonly IHelloWorldDemo _helloWorldDemo = helloWorldDemo ?? throw new ArgumentNullException(nameof(helloWorldDemo));
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
        var clientOptions = new CopilotClientOptions
        {
            LogLevel = "debug",
            Logger = _copilotLogger,
        };

        await using var client = _clientFactory.Create(clientOptions);

        var clientWrapper = new CopilotClientWrapper(client);

        try
        {
            ValidateModeSelection(args);

            if (IsHelloWorldMode(args))
            {
                await _helloWorldDemo.RunAsync(clientWrapper);
            }
            else if (IsNoStreamingMode(args))
            {
                await _nonStreamingDemo.RunAsync(clientWrapper);
            }
            else
            {
                await _streamingDemo.RunAsync(clientWrapper);
            }

            await _clientInfoLogger.LogConnectionInfoAsync(client, clientOptions);
            return 0;
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

    /// <summary>
    /// Hello World モードかどうかを判定します
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    /// <returns>Hello World モードの場合は true</returns>
    public static bool IsHelloWorldMode(string[] args)
    {
        return args.Any(arg => arg.Equals("--hello-world", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// モード選択が有効かどうかを検証します
    /// 複数のモードフラグが指定されている場合は例外をスローします
    /// </summary>
    /// <param name="args">コマンドライン引数</param>
    /// <exception cref="ArgumentException">複数のモードフラグが指定されている場合</exception>
    public static void ValidateModeSelection(string[] args)
    {
        var modeCount = 0;
        if (IsHelloWorldMode(args)) modeCount++;
        if (IsNoStreamingMode(args)) modeCount++;

        if (modeCount > 1)
        {
            throw new ArgumentException("複数のモードフラグを同時に指定することはできません。--hello-world または --no-streaming のいずれか1つを指定してください。");
        }
    }
}
