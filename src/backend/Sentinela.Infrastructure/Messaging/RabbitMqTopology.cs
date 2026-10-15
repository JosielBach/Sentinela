using RabbitMQ.Client;

namespace Sentinela.Infrastructure.Messaging;

public static class RabbitMqTopology
{
    public const string EXCHANGE = "sentinela.ingestion";
    public const string QUEUE = "sentinela.samples";
    public const string ROUTING_KEY = "samples";
    public const string DEAD_LETTER_EXCHANGE = "sentinela.ingestion.dlx";
    public const string DEAD_LETTER_QUEUE = "sentinela.samples.dlq";
    private const int DELIVERY_LIMIT = 3;

    public static async Task Declare(IChannel channel)
    {
        await channel.ExchangeDeclareAsync(DEAD_LETTER_EXCHANGE, ExchangeType.Fanout, durable: true);
        await channel.QueueDeclareAsync(DEAD_LETTER_QUEUE, durable: true, exclusive: false, autoDelete: false);
        await channel.QueueBindAsync(DEAD_LETTER_QUEUE, DEAD_LETTER_EXCHANGE, routingKey: string.Empty);

        var arguments = new Dictionary<string, object?>
        {
            ["x-queue-type"] = "quorum",
            ["x-dead-letter-exchange"] = DEAD_LETTER_EXCHANGE,
            ["x-delivery-limit"] = DELIVERY_LIMIT
        };

        await channel.ExchangeDeclareAsync(EXCHANGE, ExchangeType.Direct, durable: true);
        await channel.QueueDeclareAsync(QUEUE, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
        await channel.QueueBindAsync(QUEUE, EXCHANGE, ROUTING_KEY);
    }
}
