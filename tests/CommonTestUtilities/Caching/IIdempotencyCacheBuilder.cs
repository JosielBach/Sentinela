using Moq;
using Sentinela.Domain.Caching;

namespace CommonTestUtilities.Caching;

public class IIdempotencyCacheBuilder
{
    public static IIdempotencyCache Build(bool registered = true)
    {
        var mock = new Mock<IIdempotencyCache>();
        mock.Setup(cache => cache.TryRegister(It.IsAny<string>(), It.IsAny<TimeSpan>())).ReturnsAsync(registered);
        return mock.Object;
    }
}
