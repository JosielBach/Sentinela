using FluentValidation.Results;
using Mapster;
using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;
using Sentinela.Domain.Repositories;
using Sentinela.Domain.Repositories.Asset;
using Sentinela.Domain.Security.ApiKeyHashing;
using Sentinela.Exception;
using Sentinela.Exception.ExceptionBase;
using System.Security.Cryptography;

namespace Sentinela.Application.UseCases.Asset.Register;

public class RegisterAssetUseCase : IRegisterAssetUseCase
{
    private readonly IApiKeyHasher _apiKeyHasher;
    private readonly IAssetReadOnlyRepository _assetReadOnlyRepository;
    private readonly IAssetWriteOnlyRepository _assetWriteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    public RegisterAssetUseCase(IApiKeyHasher apiKeyHasher, IAssetWriteOnlyRepository assetWriteOnlyRepository,
        IAssetReadOnlyRepository assetReadOnlyRepository, IUnitOfWork unitOfWork)
    {
        _apiKeyHasher = apiKeyHasher;
        _assetReadOnlyRepository = assetReadOnlyRepository;
        _assetWriteOnlyRepository = assetWriteOnlyRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<ResponseRegisterAssetJson> Execute(RequestRegisterAssetJson request)
    {
        await ValidateAndThrowOnFailures(request);

        var asset = request.Adapt<Domain.Entities.Asset>();

        byte[] random = new byte[32];
        RandomNumberGenerator.Fill(random);
        string hexString = Convert.ToHexString(random).ToLower();
        var hashedKey = _apiKeyHasher.HashApiKey(hexString);
        asset.ApiKeyHash = hashedKey;

        await _assetWriteOnlyRepository.Add(asset);
        await _unitOfWork.Commit();

        return new ResponseRegisterAssetJson
        {
            Id = asset.Id,
            ApiKey = hexString
        };

    }

    private async Task ValidateAndThrowOnFailures(RequestRegisterAssetJson request)
    {
        var validator = new RegisterAssetValidator();
        var result = validator.Validate(request);

        var hostnameExist = await _assetReadOnlyRepository.ExistAssetWithHostname(request.Hostname);
        if (hostnameExist)
        {
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.VALIDATION_HOSTNAME_ALREDY_EXISTS));
        }

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
    
}
