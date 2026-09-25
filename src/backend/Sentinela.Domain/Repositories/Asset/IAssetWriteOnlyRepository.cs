namespace Sentinela.Domain.Repositories.Asset;

public interface IAssetWriteOnlyRepository
{
    Task Add(Entities.Asset asset);
}
