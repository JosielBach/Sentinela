using Sentinela.Domain.Enums;

namespace Sentinela.Domain.Entities;

public class Sample
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public required Guid AssetId { get; init; }
    public required MetricType Metric { get; init; }
    public required double Value { get; init; }
    public required DateTime CollectedAt { get; init; }
    public required DateTime ReceivedAt { get; init; }
}
