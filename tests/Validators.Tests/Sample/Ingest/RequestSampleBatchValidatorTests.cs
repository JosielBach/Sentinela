using CommonTestUtilities.Requests;
using Sentinela.Application.UseCases.Sample.Ingest;
using Sentinela.Communication.Enums;
using Sentinela.Exception;
using Shouldly;

namespace Validators.Tests.Sample.Ingest;

public class RequestSampleBatchValidatorTests
{
    [Fact]
    public void Success()
    {
        var validator = new RequestSampleBatchValidator();

        var request = RequestSampleBatchJsonBuilder.Build();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenSamplesIsEmpty()
    {
        var request = RequestSampleBatchJsonBuilder.Build();
        request.Samples.Clear();

        var validator = new RequestSampleBatchValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_SAMPLES_REQUIRED));
        });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(99)]
    public void Validate_ShouldHaveError_WhenMetricIsInvalid(int metric)
    {
        var request = RequestSampleBatchJsonBuilder.Build();
        request.Samples[0].Metric = (MetricType)metric;

        var validator = new RequestSampleBatchValidator();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldSatisfyAllConditions(errors =>
        {
            errors.Count.ShouldBe(1);
            errors.ShouldContain(error => error.ErrorMessage.Equals(ResourceMessagesException.VALIDATION_METRIC_INVALID));
        });
    }
}