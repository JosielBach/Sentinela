using Moq;
using Sentinela.Domain.Entities;
using Sentinela.Domain.Repositories.Asset;

namespace CommonTestUtilities.Repositories;

public class IAssetReadOnlyRepositoryBuilder
{
    private readonly Mock<IAssetReadOnlyRepository> _mock;
    public IAssetReadOnlyRepositoryBuilder() => _mock = new Mock<IAssetReadOnlyRepository>();

    public void ExistAssetWithHostname(string hostname)
    {
        _mock.Setup(repository => repository.ExistAssetWithHostname(hostname)).ReturnsAsync(true);
    }
    public void GetAll(int page, int pageSize, IList<Asset> assets)
    {
        _mock.Setup(repository => repository.GetAll(page, pageSize)).ReturnsAsync(assets);
    }

    public void CountAll(int totalCount)
    {
        _mock.Setup(repository => repository.CountAll()).ReturnsAsync(totalCount);
    }
    public IAssetReadOnlyRepository Build() => _mock.Object;
}
