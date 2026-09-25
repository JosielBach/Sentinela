using Sentinela.Domain.Security.ApiKeyHashing;
using System.Security.Cryptography;
using System.Text;

namespace Sentinela.Infrastructure.Security.ApiKeyHashing;

internal sealed class Sha256ApiKeyHasher : IApiKeyHasher
{
    public string HashApiKey(string apiKey)
    {
        var apiKeyBytes = Encoding.UTF8.GetBytes(apiKey);

        var apiKeyHashed = SHA256.HashData(apiKeyBytes);

        return Convert.ToHexString(apiKeyHashed);
    }
}
