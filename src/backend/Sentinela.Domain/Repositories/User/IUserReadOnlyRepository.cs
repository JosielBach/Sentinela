namespace Sentinela.Domain.Repositories.User;

public interface IUserReadOnlyRepository
{
    Task<bool> ExistActiveUserWithEmail(string email);
    Task<bool> ExistActiveUserWithId(Guid userId);
    Task<Entities.User?> GetByEmail(string email);
}
