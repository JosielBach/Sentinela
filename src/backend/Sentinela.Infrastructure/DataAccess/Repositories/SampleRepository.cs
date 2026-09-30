using Sentinela.Domain.Entities;
using Sentinela.Domain.Repositories.Sample;

namespace Sentinela.Infrastructure.DataAccess.Repositories;

internal sealed class SampleRepository : ISampleWriteOnlyRepository
{
    private readonly SentinelaDbContext _dbContext;
    public SampleRepository(SentinelaDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task AddRange(IList<Sample> samples)
    {
        await _dbContext.Samples.AddRangeAsync(samples);
    }
}
