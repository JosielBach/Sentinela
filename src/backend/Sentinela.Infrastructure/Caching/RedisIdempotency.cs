using Sentinela.Domain.Caching;
using StackExchange.Redis;

namespace Sentinela.Infrastructure.Caching;

internal sealed class RedisIdempotency : IIdempotencyCache
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    public RedisIdempotency(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task Remove(string key)
    {
        var database = _connectionMultiplexer.GetDatabase();

        await database.KeyDeleteAsync(key);
    }

    public async Task<bool> TryRegister(string key, TimeSpan timeToLive)
    {
        var database = _connectionMultiplexer.GetDatabase();

        var result = await database.StringSetAsync(key, "1", timeToLive, When.NotExists);

        return result;
    }
}