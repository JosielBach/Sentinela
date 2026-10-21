using Sentinela.Communication.Requests;

namespace Sentinela.Agent.Metrics;

internal interface IMetricsCollector
{
    IList<RequestSampleJson> Collect();
}
