using CommonTestUtilities.Requests;
using Sentinela.Application.UseCases.Asset.Register;
using Sentinela.Communication.Enums;
using Sentinela.Exception;
using Shouldly;
using System.Diagnostics.CodeAnalysis;

namespace Validators.Tests.Asset.Register;

public class RegisterAssetValidatorTests
{
    [Fact]
    public void Success()
    {
        var validator = new RegisterAssetValidator();

        var request = RequestRegisterAssetJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("          ")]
    [SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "Intencional por ser um teste unitário.")]
    public void Validate_ShouldHaveError_WhenHostnameIsEmpty(string hostname)
    {
        var request = RequestRegisterAssetJsonBuilder.Build();
        request.Hostname = hostname;

        var validator = new RegisterAssetValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_HOSTNAME_REQUIRED));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenTypeIsInvalid()
    {
        var request = RequestRegisterAssetJsonBuilder.Build();
        request.Type = (AssetType)99;

        var validator = new RegisterAssetValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_ASSET_TYPE_INVALID));
        });
    }
}
