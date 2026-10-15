using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Sentinela.Application.UseCases.Sample;
using Sentinela.Application.UseCases.Sample.Process;
using Sentinela.Infrastructure.Messaging;
using System.Text.Json;

namespace Sentinela.Worker.Workers;

public class IngestionConsumerWorker : BackgroundService
{
    private const ushort PREFETCH_COUNT = 10;
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<IngestionConsumerWorker> _logger;
    public IngestionConsumerWorker(IConnection connection, IServiceScopeFactory serviceScopeFactory,
            ILogger<IngestionConsumerWorker> logger)
    {
        _connection = connection;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await RabbitMqTopology.Declare(channel);

        await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: PREFETCH_COUNT, global: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var message = JsonSerializer.Deserialize<SampleBatchMessage>(eventArgs.Body.Span)!;

                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var useCase = scope.ServiceProvider.GetRequiredService<IProcessSampleBatchUseCase>();

                await useCase.Execute(message);

                await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            }
            catch (JsonException exception)
            {
                _logger.LogError(exception, "Invalid sample batch message");

                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
            }
            catch (System.Exception exception)
            {
                _logger.LogError(exception, "Failed to process sample batch");

                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(RabbitMqTopology.QUEUE, autoAck: false, consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
