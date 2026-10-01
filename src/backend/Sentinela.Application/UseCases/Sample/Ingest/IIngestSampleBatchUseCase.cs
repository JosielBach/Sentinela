using Sentinela.Communication.Requests;

namespace Sentinela.Application.UseCases.Sample.Ingest;

public interface IIngestSampleBatchUseCase 
{
    Task Execute(RequestSampleBatchJson request, Guid assetId, string idempotencyKey);
}
