using Moq;
using Sentinela.Domain.Repositories.Sample;

namespace CommonTestUtilities.Repositories;

public class ISampleWriteOnlyRepositoryBuilder
{
    public static ISampleWriteOnlyRepository Build()
    {
        var mock = new Mock<ISampleWriteOnlyRepository>();
        return mock.Object;
    }
}
