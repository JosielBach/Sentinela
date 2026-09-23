using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using Sentinela.Application.UseCases.User.Register;
using Sentinela.Domain.Extensions;
using Sentinela.Exception;
using Sentinela.Exception.ExceptionBase;
using Shouldly;

namespace UseCases.Tests.User.Register;

public class RegisterUserAccountUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build();

        var useCase = CreateUseCase();

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Tokens.ShouldNotBeNull();
        result.Name.ShouldBe(request.Name);
        result.Tokens.AccessToken.ShouldNotBeNullOrEmpty();
        result.Tokens.RefreshToken.ShouldBeNullOrEmpty();
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenNameIsEmpty()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build();
        request.Name = string.Empty;

        var useCase = CreateUseCase();

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_NAME_REQUIRED);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenEmailAlreadyExists()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build();

        var useCase = CreateUseCase(request.Email);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_EMAIL_ALREDY_EXISTS);
        });
    }

    private RegisterUserAccountUseCase CreateUseCase(string? emailThatAlreadyExists = null)
    {
        var accessTokenGeneratorBuilder = IAccessTokenGeneratorBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var userWriteOnlyRepository = IUserWriteOnlyRepositoryBuilder.Build();
        var userReadOnlyRepository = new IUserReadOnlyRepositoryBuilder();
        if (emailThatAlreadyExists.IsNotEmpty())
        {
            userReadOnlyRepository.ExistActiveUserWithEmail(emailThatAlreadyExists);
        }
        var passwordHasher = new IPasswordHasherBuilder().Build();

        return new RegisterUserAccountUseCase(passwordHasher, userWriteOnlyRepository, unitOfWork, userReadOnlyRepository.Build()
            , accessTokenGeneratorBuilder);
    }
}
