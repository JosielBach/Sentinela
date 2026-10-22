using Sentinela.Agent.Configuration;
using Sentinela.Agent.Metrics;
using Sentinela.Communication.Requests;
using System.Net.Http.Json;

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
                string idempotencyKey = Guid.NewGuid().ToString();
                var samples = _metricsCollector.Collect();
                RequestSampleBatchJson batch = new RequestSampleBatchJson
                {
                    Samples = samples
                };
                _logger.LogInformation("Lote: {IdempotencyKey} Amostras: {SampleCount}", idempotencyKey, samples.Count);
                await SendBatch(batch, idempotencyKey, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Failed to collect or send sample batch");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
    private async Task SendBatch(RequestSampleBatchJson batch, string idempotencyKey, CancellationToken cancellationToken)
    {
        var httpClient = _httpClientFactory.CreateClient("Sentinela");
        
        using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/ingest");
        request.Headers.Add("Idempotency-Key", idempotencyKey);

        request.Content = JsonContent.Create(batch);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Lote enviado com sucesso: {IdempotencyKey}", idempotencyKey);
        }
        else if ((int)response.StatusCode >= 500)
        {
            _logger.LogWarning("API indisponível. Lote: {IdempotencyKey}. StatusCode: {StatusCode}", idempotencyKey, response.StatusCode);
        }
        else
        {
            _logger.LogError("Falha ao enviar lote: {IdempotencyKey}. StatusCode: {StatusCode}", idempotencyKey, response.StatusCode);
        }
    }
}