using Sentinela.Communication.Enums;

namespace Sentinela.Communication.Requests;

public class RequestSampleJson
{
    public MetricType Metric { get; set; }
    public double Value { get; set; }
    public DateTime CollectedAt { get; set; }
}
