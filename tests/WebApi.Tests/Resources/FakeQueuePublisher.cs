using Sentinela.Domain.Messaging;

namespace WebApi.Tests.Resources;

public class FakeQueuePublisher : IQueuePublisher
{
    public List<object> Messages { get; } = [];

    public Task Publish<T>(T message)
    {
        Messages.Add(message!);
        return Task.CompletedTask;
    }
}
