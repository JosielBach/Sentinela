namespace Sentinela.Domain.Caching;

public interface IIdempotencyCache
{
    Task<bool> TryRegister(string key, TimeSpan timeToLive);

    Task Remove(string key);
}
