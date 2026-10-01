using Sentinela.Communication.Requests;

namespace Sentinela.Application.UseCases.Sample;

public record SampleBatchMessage
{
    public Guid AssetId { get; init; }
    public string IdempotencyKey { get; init; }
    public DateTime ReceivedAt { get; init; }
    public IList<RequestSampleJson> Samples { get; init; }
    public SampleBatchMessage(Guid assetId, string idempotencyKey, DateTime receivedAt, IList<RequestSampleJson> samples)
    {
        AssetId = assetId;
        IdempotencyKey = idempotencyKey;
        ReceivedAt = receivedAt;
        Samples = samples;
    }
}
