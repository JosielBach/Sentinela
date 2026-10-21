using Sentinela.Agent.Configuration;
using Sentinela.Agent.Metrics;

namespace Sentinela.Agent.Workers;

internal class BatchSenderService : BackgroundService
{
    private readonly IMetricsCollector _metricsCollector;
    private readonly AgentConfiguration _agentConfiguration;
    private readonly ILogger<BatchSenderService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    public BatchSenderService(IMetricsCollector metricsCollector, AgentConfiguration agentConfiguration,
        ILogger<BatchSenderService> logger, IHttpClientFactory httpClientFactory)
    {
        _metricsCollector = metricsCollector;
        _agentConfiguration = agentConfiguration;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_agentConfiguration.Interval);
        do 
        {
            try
            {
                var metrics = _metricsCollector.Collect();
                foreach (var sample in metrics)
                {
                    _logger.LogInformation($"{sample.Metric}: {sample.Value:F1}");
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to collect metrics");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
