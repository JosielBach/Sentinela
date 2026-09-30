namespace Sentinela.Domain.Repositories.Sample;

public interface ISampleWriteOnlyRepository
{
    Task AddRange(IList<Entities.Sample> samples);
}
