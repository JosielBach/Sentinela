using Sentinela.Communication.Requests;
using Sentinela.Domain.Messaging;
using Sentinela.Exception.ExceptionBase;

namespace Sentinela.Application.UseCases.Sample.Ingest;

public class IngestSampleBatchUseCase : IIngestSampleBatchUseCase
{
    private readonly IQueuePublisher _queuePublisher;

    public IngestSampleBatchUseCase(IQueuePublisher queuePublisher)
    {
        _queuePublisher = queuePublisher;
    }

    public async Task Execute(RequestSampleBatchJson request, Guid assetId, string idempotencyKey)
    {
        ValidateRequest(request);

        var message = new SampleBatchMessage(assetId, idempotencyKey, DateTime.UtcNow, request.Samples);

        await _queuePublisher.Publish(message);
    }

    private void ValidateRequest(RequestSampleBatchJson request)
    {
        var validator = new RequestSampleBatchValidator();
        var result = validator.Validate(request);

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
