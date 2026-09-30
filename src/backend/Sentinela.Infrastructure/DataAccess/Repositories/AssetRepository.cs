using Microsoft.EntityFrameworkCore;
using Sentinela.Domain.Entities;
using Sentinela.Domain.Repositories.Asset;

namespace Sentinela.Infrastructure.DataAccess.Repositories;

internal sealed class AssetRepository : IAssetReadOnlyRepository, IAssetWriteOnlyRepository, IAssetUpdateOnlyRepository
{
    private readonly SentinelaDbContext _dbContext;
    public AssetRepository(SentinelaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Add(Asset asset) => await _dbContext.Assets.AddAsync(asset);

    public async Task<int> CountAll()
    {
        return await _dbContext.Assets.CountAsync();
    }

    public async Task<IList<Asset>> GetAll(int page, int pageSize)
    {
        return await _dbContext.Assets.AsNoTracking().OrderBy(asset => asset.Id)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<bool> ExistAssetWithHostname(string hostname)
    {
        return await _dbContext.Assets.AnyAsync(asset => asset.Active && asset.Hostname.Equals(hostname));
    }

    public async Task<Asset?> GetById(Guid assetId)
    {
        return await _dbContext.Assets.AsNoTracking().SingleOrDefaultAsync(asset => asset.Id == assetId);
    }

    public async Task<Asset?> GetByApiKeyHash(string apiKeyHash)
    {
        return await _dbContext.Assets.AsNoTracking().SingleOrDefaultAsync(asset => asset.ApiKeyHash == apiKeyHash);
    }

    public async Task UpdateLastSeen(Guid assetId, DateTime lastSeenAt)
    {
        var asset = await _dbContext.Assets.FirstOrDefaultAsync(asset => asset.Id == assetId);
        if (asset != null)
        {
            asset.LastSeenAt = lastSeenAt;
        }
    }
}
