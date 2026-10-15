using RabbitMQ.Client;
using Sentinela.Domain.Messaging;
using System.Text.Json;

namespace Sentinela.Infrastructure.Messaging;

internal sealed class RabbitMqPublisher : IQueuePublisher
{
    private readonly IConnection _connection;
    public RabbitMqPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public async Task Publish<T>(T message)
    {
        var options = new CreateChannelOptions(publisherConfirmationsEnabled: true, publisherConfirmationTrackingEnabled: true);

        await using var channel = await _connection.CreateChannelAsync(options);

        await RabbitMqTopology.Declare(channel);

        var properties = new BasicProperties { Persistent = true, ContentType = "application/json" };
        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        await channel.BasicPublishAsync(RabbitMqTopology.EXCHANGE, RabbitMqTopology.ROUTING_KEY, mandatory: true, properties, body);
    }
}
