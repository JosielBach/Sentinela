using Moq;
using Sentinela.Domain.Messaging;

namespace CommonTestUtilities.Messaging;

public class IQueuePublisherBuilder
{
    private readonly Mock<IQueuePublisher> _mock;
    public IQueuePublisherBuilder() => _mock = new Mock<IQueuePublisher>();
    public Mock<IQueuePublisher> Mock => _mock;
    public IQueuePublisher Build() => _mock.Object;

}
