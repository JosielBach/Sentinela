namespace Sentinela.Domain.Security.ApiKeyHashing;

public interface IApiKeyHasher
{
    string HashApiKey(string apiKey);
}
