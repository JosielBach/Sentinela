namespace Sentinela.Domain.Messaging;

public interface IQueuePublisher
{
    Task Publish<T>(T message);
}
