namespace Sentinela.Domain.Repositories;

public interface IUnitOfWork
{
    Task Commit();
}
