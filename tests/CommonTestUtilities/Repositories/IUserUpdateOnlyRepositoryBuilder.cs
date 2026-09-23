using Moq;
using Sentinela.Domain.Repositories.User;

namespace CommonTestUtilities.Repositories;

public class IUserUpdateOnlyRepositoryBuilder
{
    public static IUserUpdateOnlyRepository Builder()
    {
        var mock = new Mock<IUserUpdateOnlyRepository>();

        return mock.Object;
    }

}
