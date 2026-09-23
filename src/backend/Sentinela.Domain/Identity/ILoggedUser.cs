using Sentinela.Domain.Entities;

namespace Sentinela.Domain.Identity;

public interface ILoggedUser
{
    Task<User> Get();
    Guid GetUserId();
}
