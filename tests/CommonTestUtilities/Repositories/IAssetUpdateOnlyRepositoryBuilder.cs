using Moq;
using Sentinela.Domain.Entities;
using Sentinela.Domain.Repositories.Asset;

namespace CommonTestUtilities.Repositories;

public class IAssetUpdateOnlyRepositoryBuilder
{
    public static IAssetUpdateOnlyRepository Build()
    {
        var mock = new Mock<IAssetUpdateOnlyRepository>();
        return mock.Object;
    }
}
