using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using CommonTestUtilities.Security;
using Sentinela.Application.UseCases.Asset.Register;
using Sentinela.Communication.Enums;
using Sentinela.Domain.Extensions;
using Sentinela.Exception;
using Sentinela.Exception.ExceptionBase;
using Shouldly;

namespace UseCases.Tests.Asset.Register;

public class RegisterAssetUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterAssetJsonBuilder.Build();

        var useCase = CreateUseCase();

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.ApiKey.ShouldNotBeNullOrEmpty();
        result.ApiKey.Length.ShouldBe(64);
        result.ApiKey.ShouldNotBe("hashedKey");
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenHostnameAlreadyExists()
    {
        var request = RequestRegisterAssetJsonBuilder.Build();

        var useCase = CreateUseCase(request.Hostname);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_HOSTNAME_ALREDY_EXISTS);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenHostnameIsEmpty()
    {
        var request = RequestRegisterAssetJsonBuilder.Build();
        request.Hostname = string.Empty;

        var useCase = CreateUseCase();

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_HOSTNAME_REQUIRED);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenTypeIsInvalid()
    {
        var request = RequestRegisterAssetJsonBuilder.Build();
        request.Type = (AssetType)99;

        var useCase = CreateUseCase();

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_ASSET_TYPE_INVALID);
        });
    }


    private RegisterAssetUseCase CreateUseCase(string? hostnameThatAlreadyExists = null)
    {
        var apiKeyHasher = new IApiKeyHasherBuilder().Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var assetWriteOnlyRepository = IAssetWriteOnlyRepositoryBuilder.Build();
        var assetReadOnlyRepository = new IAssetReadOnlyRepositoryBuilder();
        if (hostnameThatAlreadyExists.IsNotEmpty())
        {
            assetReadOnlyRepository.ExistAssetWithHostname(hostnameThatAlreadyExists);
        }
        return new RegisterAssetUseCase(apiKeyHasher, assetWriteOnlyRepository, assetReadOnlyRepository.Build(), unitOfWork);
    }
}
