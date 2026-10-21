using Sentinela.Agent.Configuration;
using Sentinela.Agent.Metrics;
using Sentinela.Agent.Workers;

var builder = Host.CreateApplicationBuilder(args);

if (OperatingSystem.IsWindows())
{
    builder.Services.AddSingleton<IMetricsCollector, WindowsMetricsCollector>();
}
else if (OperatingSystem.IsLinux())
{
    builder.Services.AddSingleton<IMetricsCollector, LinuxMetricsCollector>();
}

builder.Services.AddHostedService<BatchSenderService>();

var agentConfiguration = builder.Configuration.GetSection("Agent").Get<AgentConfiguration>()!;
builder.Services.AddSingleton(agentConfiguration);

builder.Services.AddHttpClient("Sentinela", client =>
{
    client.BaseAddress = new Uri(agentConfiguration.ApiUrl);
    client.DefaultRequestHeaders.Add("X-Agent-Key", agentConfiguration.ApiKey);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Sentinela.Agent";
});
builder.Services.AddSystemd();

var host = builder.Build();

host.Run();
