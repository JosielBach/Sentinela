using CommonTestUtilities.Requests;
using Sentinela.Application.UseCases.Asset.ListAssets;
using Sentinela.Exception;
using Shouldly;

namespace Validators.Tests.Asset.ListAssets;

public class ListAssetsValidatorTests
{
    [Fact]
    public void Success()
    {
        var validator = new ListAssetsValidator();

        var request = RequestListAssetsJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Success_WhenPageSizeIsOnTheLimit(int pageSize)
    {
        var request = RequestListAssetsJsonBuilder.Build();
        request.PageSize = pageSize;

        var validator = new ListAssetsValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldHaveError_WhenPageIsInvalid(int page)
    {
        var request = RequestListAssetsJsonBuilder.Build();
        request.Page = page;

        var validator = new ListAssetsValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PAGE_INVALID));
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_ShouldHaveError_WhenPageSizeIsInvalid(int pageSize)
    {
        var request = RequestListAssetsJsonBuilder.Build();
        request.PageSize = pageSize;

        var validator = new ListAssetsValidator();
        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_PAGE_SIZE_INVALID));
        });
    }
}
