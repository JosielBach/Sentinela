namespace Sentinela.Domain.Repositories.Asset;

public interface IAssetUpdateOnlyRepository
{
    Task UpdateLastSeen(Guid assetId, DateTime lastSeenAt);
}