using GitHub.Copilot.SDK;
using Microsoft.Extensions.Logging;

namespace CopilotSdkPlayground.E2ETests;

/// <summary>
/// <see cref="CopilotClient"/> 接続用フィクスチャ
/// テストクラス間で共有するクライアント接続を管理します
/// </summary>
public class CopilotClientFixture : IAsyncLifetime
{
    private CopilotClient? _client;
    private readonly ILoggerFactory _loggerFactory;

    public CopilotClient Client => _client ?? throw new InvalidOperationException("Client not initialized");
    public bool IsAvailable { get; private set; }
    public string? SkipReason { get; private set; }

    public CopilotClientFixture()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Debug);
            builder.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
        });
    }

    public async Task InitializeAsync()
    {
        // GH_TOKEN or GITHUB_TOKEN is used by Copilot CLI for authentication
        // See: https://docs.github.com/en/copilot/how-tos/set-up/install-copilot-cli#authenticating-with-a-personal-access-token
        var authToken = Environment.GetEnvironmentVariable("GH_TOKEN")
            ?? Environment.GetEnvironmentVariable("GITHUB_TOKEN");

        if (string.IsNullOrEmpty(authToken))
        {
            SkipReason = "GH_TOKEN or GITHUB_TOKEN environment variable is not set. E2E tests require authentication.";
            IsAvailable = false;
            return;
        }

        try
        {
            var logger = _loggerFactory.CreateLogger<CopilotClient>();
            _client = new CopilotClient(new CopilotClientOptions
            {
                LogLevel = "debug",
                Logger = logger,
            });

            IsAvailable = true;
        }
        catch (Exception ex)
        {
            SkipReason = $"Failed to initialize CopilotClient: {ex.Message}";
            IsAvailable = false;
        }
    }

    public async Task DisposeAsync()
    {
        if (_client != null)
        {
            await _client.DisposeAsync();
        }
        _loggerFactory.Dispose();
    }
}
