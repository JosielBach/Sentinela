using FluentValidation;
using Sentinela.Communication.Requests;
using Sentinela.Exception;

namespace Sentinela.Application.UseCases.Asset.Register;

public class RegisterAssetValidator : AbstractValidator<RequestRegisterAssetJson>
{
    public RegisterAssetValidator()
    {
        RuleFor(asset => asset.Hostname).NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_HOSTNAME_REQUIRED);
        RuleFor(asset => asset.Type).IsInEnum().WithMessage(ResourceMessagesException.VALIDATION_ASSET_TYPE_INVALID);
    }
}
