using CommonTestUtilities.Entities;
using CommonTestUtilities.Identity;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Sentinela.Application.UseCases.User.Update;
using Sentinela.Domain.Extensions;
using Sentinela.Exception;
using Sentinela.Exception.ExceptionBase;
using Shouldly;

namespace UseCases.Tests.User.UpdateProfile;

public class UpdateUserUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user);

        await useCase.Execute(request).ShouldNotThrowAsync();

        user.Name.ShouldBe(request.Name);
        user.Email.ShouldBe(request.Email);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenNameIsEmpty()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var useCase = CreateUseCase(user);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_NAME_REQUIRED);
        });

        user.Name.ShouldNotBe(request.Name);
        user.Email.ShouldNotBe(request.Email);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenEmailAlreadyExists()
    {
        (var user, _) = UserBuilder.Build();

        var request = RequestUpdateUserJsonBuilder.Build();

        var useCase = CreateUseCase(user, request.Email);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_EMAIL_ALREDY_EXISTS);
        });

        user.Email.ShouldNotBe(request.Email);
    }

    private UpdateUserUseCase CreateUseCase(Sentinela.Domain.Entities.User user, string? emailThatAlreadyExists = null)
    {
        var unitOfWOrk = IUnitOfWorkBuilder.Build();
        var userUpdateRepository = IUserUpdateOnlyRepositoryBuilder.Builder();
        var loggedUser = ILoggedUserBuilder.Build(user);

        var userReadOnlyRepositoryBuilder = new IUserReadOnlyRepositoryBuilder();
        if (emailThatAlreadyExists.IsNotEmpty())
            userReadOnlyRepositoryBuilder.ExistActiveUserWithEmail(emailThatAlreadyExists);

        return new UpdateUserUseCase(loggedUser, userReadOnlyRepositoryBuilder.Build(), userUpdateRepository, unitOfWOrk);
    }
}
