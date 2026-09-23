using Moq;
using Sentinela.Domain.Entities;
using Sentinela.Domain.Identity;

namespace CommonTestUtilities.Identity;

public class ILoggedUserBuilder
{
    public static ILoggedUser Build(User user)
    {
        var mock = new Mock<ILoggedUser>();

        mock.Setup(loggedUser => loggedUser.Get()).ReturnsAsync(user);

        mock.Setup(loggedUser => loggedUser.GetUserId()).Returns(user.Id);

        return mock.Object;
    }
}
