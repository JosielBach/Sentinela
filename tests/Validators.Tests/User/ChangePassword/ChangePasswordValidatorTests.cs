using CommonTestUtilities.Requests;
using Sentinela.Application.UseCases.User.ChangePassword;
using Sentinela.Exception;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Text;

namespace Validators.Tests.User.ChangePassword;

public class ChangePasswordValidatorTests
{
    [Fact]
    public void Success()
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Builder();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldValidate_ShouldHaveError_WhenPasswordIsEmpty()
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Builder();
        request.NewPassword = string.Empty;

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_REQUIRED));
        });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    public void Validate_ShouldValidate_ShouldHaveError_WhenPasswordIsInvalid(int passwordLength)
    {
        var validator = new ChangePasswordValidator();

        var request = RequestChangePasswordJsonBuilder.Builder(passwordLength);

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PASSWORD_LENGTH));
        });
    }
}
