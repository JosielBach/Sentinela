using Moq;
using Sentinela.Domain.Repositories;

namespace CommonTestUtilities.Repositories;

public class IUnitOfWorkBuilder
{
    public static IUnitOfWork Build()
    {
        var mock = new Mock<IUnitOfWork>();
        return mock.Object;
    }
    public static void VerifyCommit(IUnitOfWork unitOfWork)
    {
        Mock.Get(unitOfWork).Verify(uow => uow.Commit(), Times.Once);
    }
}
