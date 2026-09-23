using Bogus;
using Moq;
using Sentinela.Domain.Entities;
using Sentinela.Domain.Security.Tokens;

namespace CommonTestUtilities.Security;

public interface IAccessTokenGeneratorBuilder 
{
    public static IAccessTokenGenerator Build()
    {
        var mock = new Mock<IAccessTokenGenerator>();

        var fakeToken = new Faker().Random.String2(32, "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789");
        mock.Setup(generator => generator.Generate(It.IsAny<User>())).Returns(fakeToken);

        return mock.Object;
    }
}
