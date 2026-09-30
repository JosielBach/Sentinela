using Moq;
using Sentinela.Domain.Security.ApiKeyHashing;

namespace CommonTestUtilities.Security;

public class IApiKeyHasherBuilder
{
    private readonly Mock<IApiKeyHasher> _mock;
    public IApiKeyHasherBuilder()
    {
        _mock = new Mock<IApiKeyHasher>();

        _mock.Setup(keyHashed => keyHashed.HashApiKey(It.IsAny<string>())).Returns("hashedKey");
    }

    public IApiKeyHasher Build() => _mock.Object;
}
