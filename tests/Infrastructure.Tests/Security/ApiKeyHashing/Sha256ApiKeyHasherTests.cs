using Sentinela.Infrastructure.Security.ApiKeyHashing;
using Shouldly;

namespace Infrastructure.Tests.Security.ApiKeyHashing;
public class Sha256ApiKeyHasherTests
{
    [Fact]
    public void Success()
    {
        var apiKey = "minha-chave-secreta-de-testes";
        var hasher = new Sha256ApiKeyHasher();

        var result = hasher.HashApiKey(apiKey);

        result.ShouldNotBeNullOrEmpty();
        result.Length.ShouldBe(64);
        result.ShouldNotBe(apiKey);
    }

    [Fact]
    public void Success_ShouldReturnSameHash_WhenApiKeyIsTheSame()
    {
        var apiKey = "minha-chave-secreta-de-testes";

        var apiKey2 = "minha-chave-secreta-de-testes";
        var hasher = new Sha256ApiKeyHasher();

        var result = hasher.HashApiKey(apiKey);
        var result2 = hasher.HashApiKey(apiKey2);

        result.ShouldBe(result2);
    }

    [Fact]
    public void Success_ShouldReturnDifferentHash_WhenApiKeyIsDifferent()
    {
        var apiKey = "minha-chave-secreta-de-testes";

        var apiKey2 = "minha-chave-de-testes";
        var hasher = new Sha256ApiKeyHasher();

        var result = hasher.HashApiKey(apiKey);
        var result2 = hasher.HashApiKey(apiKey2);

        result.ShouldNotBe(result2);
    }

    [Fact]
    public void Success_ShouldReturnKnownHash_WhenApiKeyIsAbc()
    {
        var apiKey = "abc";
        var hasher = new Sha256ApiKeyHasher();

        var result = hasher.HashApiKey(apiKey);

        result.Length.ShouldBe(64);
        result.ShouldBe("BA7816BF8F01CFEA414140DE5DAE2223B00361A396177A9CB410FF61F20015AD");
    }
}

