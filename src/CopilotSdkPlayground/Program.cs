using CopilotSdkPlayground;
using CopilotSdkPlayground.Abstractions;
using CopilotSdkPlayground.Demos;
using CopilotSdkPlayground.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "HH:mm:ss ";
});

builder.Services.AddSingleton<ICopilotClientFactory, CopilotClientFactory>();
builder.Services.AddSingleton<IEnvironmentProvider, EnvironmentProvider>();
builder.Services.AddSingleton<IFileSystem, FileSystemWrapper>();
builder.Services.AddSingleton<IConsoleWriter, ConsoleWriter>();
builder.Services.AddSingleton<ICopilotClientInfoLogger, CopilotClientInfoLoggerService>();
builder.Services.AddSingleton<IStreamingDemo, StreamingDemoService>();
builder.Services.AddSingleton<INonStreamingDemo, NonStreamingDemoService>();
builder.Services.AddSingleton<App>();

using var host = builder.Build();
var app = host.Services.GetRequiredService<App>();
return await app.RunAsync(args);
