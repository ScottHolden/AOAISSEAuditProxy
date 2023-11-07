using AOAISSEProxy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAuditLog, LoggerAuditLog>();
builder.Services.AddSingleton<IBackendClientFactory, FixedBackendClientFactory>();
builder.Services.AddSingleton<SSELoggerProxyTerminalMiddleware>();

var app = builder.Build();

app.UseTerminatingMiddleware<SSELoggerProxyTerminalMiddleware>();
app.Run();
