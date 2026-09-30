using Moq;
using Sentinela.Domain.Repositories.Asset;

namespace CommonTestUtilities.Repositories;

public class IAssetWriteOnlyRepositoryBuilder
{
    public static IAssetWriteOnlyRepository Build()
    {
        var mock = new Mock<IAssetWriteOnlyRepository>();
        return mock.Object;
    }
}
