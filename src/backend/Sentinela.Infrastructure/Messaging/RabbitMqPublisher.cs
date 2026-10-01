using RabbitMQ.Client;
using Sentinela.Domain.Messaging;
using System.Text.Json;

namespace Sentinela.Infrastructure.Messaging;

internal sealed class RabbitMqPublisher : IQueuePublisher
{
    private const string EXCHANGE = "sentinela.ingestion";
    private const string QUEUE = "sentinela.samples";
    private const string ROUTING_KEY = "samples";
    private readonly IConnection _connection;
    public RabbitMqPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public async Task Publish<T>(T message)
    {
        var options = new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true);

        await using var channel = await _connection.CreateChannelAsync(options);

        await channel.ExchangeDeclareAsync(EXCHANGE, ExchangeType.Direct, durable: true);
        await channel.QueueDeclareAsync(QUEUE, durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync(QUEUE, EXCHANGE, ROUTING_KEY);

        var properties = new BasicProperties { Persistent = true, ContentType = "application/json" };
        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        await channel.BasicPublishAsync(EXCHANGE, ROUTING_KEY, mandatory: true, properties, body);
    }
}
