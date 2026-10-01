using FluentValidation;
using Sentinela.Communication.Requests;
using Sentinela.Exception;

namespace Sentinela.Application.UseCases.Sample.Ingest;

public class RequestSampleBatchValidator : AbstractValidator<RequestSampleBatchJson>
{
    public RequestSampleBatchValidator()
    {
        RuleFor(request => request.Samples).NotEmpty().WithMessage(ResourceMessagesException.VALIDATION_SAMPLES_REQUIRED);
        RuleForEach(request => request.Samples).ChildRules(sample =>
        {
            sample.RuleFor(item => item.Metric).IsInEnum().WithMessage(ResourceMessagesException.VALIDATION_METRIC_INVALID);
        });
    }
}
