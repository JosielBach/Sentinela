using Bogus.DataSets;
using CommonTestUtilities.Requests;
using Sentinela.Application.UseCases.User.Update;
using Sentinela.Exception;
using Shouldly;
using System.Diagnostics.CodeAnalysis;

namespace Validators.Tests.User.Update;

public class UpdateUserValidatorTests
{
    [Fact]
    public void Success()
    {
        var validator = new UpdateUserValidator();

        var request = RequestUpdateUserJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("           ")]
    [SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "Intencional por ser um teste unitário.")]
    public void Validate_ShouldHaveError_WhenNameIsEmpty(string name)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = name;

        var validator = new UpdateUserValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_NAME_REQUIRED));
        });
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("           ")]
    [SuppressMessage("Usage", "xUnit1012:Null should only be used for nullable parameters", Justification = "Intencional por ser um teste unitário.")]
    public void Validate_ShouldHaveError_WhenEmailIsEmpty(string email)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = email;

        var validator = new UpdateUserValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_REQUIRED));
        });
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenEmailIsInvalid()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Email = "teste.com";

        var validator = new UpdateUserValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_EMAIL_INVALID));
        });
    }
}
