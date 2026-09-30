namespace Sentinela.Domain.Repositories.Asset;

public interface IAssetReadOnlyRepository
{
    Task<IList<Entities.Asset>> GetAll(int page, int pageSize);
    Task<Entities.Asset?> GetById(Guid assetId);
    Task<bool> ExistAssetWithHostname(string hostname);
    Task<int> CountAll();
    Task<Entities.Asset?> GetByApiKeyHash(string apiKeyHash);
}
