using Sentinela.Domain.Repositories;

namespace Sentinela.Infrastructure.DataAccess;

internal class UnitOfWork : IUnitOfWork
{
    private readonly SentinelaDbContext _dbContext;
    public UnitOfWork(SentinelaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task Commit() => await _dbContext.SaveChangesAsync();
}
