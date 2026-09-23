using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using Sentinela.Application.UseCases.User.ChangePassword;
using Sentinela.Exception;
using Sentinela.Exception.ExceptionBase;
using Shouldly;

namespace UseCases.Tests.User.ChangePassword;

public class ChangePasswordUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        (var user, var password) = UserBuilder.Build();

        var request = RequestChangePasswordJsonBuilder.Builder();
        request.CurrentPassword = password;

        var useCase = CreateUseCase(user, password);

        await useCase.Execute(request).ShouldNotThrowAsync();
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenPasswordIsEmpty()
    {
        (var user, var password) = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Builder();
        request.CurrentPassword = string.Empty;

        var useCase = CreateUseCase(user, password);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_CURRENT_PASSWORD);
        });

        user.Password.ShouldNotBe(request.NewPassword);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenPasswordDoesNotMatch()
    {
        (var user, var password) = UserBuilder.Build();
        var request = RequestChangePasswordJsonBuilder.Builder();
        request.CurrentPassword = "string.Empty";

        var useCase = CreateUseCase(user, password);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_CURRENT_PASSWORD);
        });

        user.Password.ShouldNotBe(request.NewPassword);
    }

    private static ChangePasswordUseCase CreateUseCase(Sentinela.Domain.Entities.User user, string password)
    {
        var userUpdateRepository = IUserUpdateOnlyRepositoryBuilder.Builder();
        var loggedUser = ILoggedUserBuilder.Build(user);
        var passwordHasher = new IPasswordHasherBuilder().VerifyPassword(password).Build();

        return new ChangePasswordUseCase(loggedUser, passwordHasher, userUpdateRepository);

    }
}
