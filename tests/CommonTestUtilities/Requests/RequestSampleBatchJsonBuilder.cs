using Bogus;
using Sentinela.Communication.Enums;
using Sentinela.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestSampleBatchJsonBuilder
{
    public static RequestSampleBatchJson Build()
    {
        return new RequestSampleBatchJson
        {
            Samples = new Faker<RequestSampleJson>()
                .RuleFor(request => request.Metric, f => f.PickRandom<MetricType>())
                .RuleFor(request => request.Value, f => f.Random.Double(0, 100))
                .RuleFor(request => request.CollectedAt, f => f.Date.Recent())
                .Generate(5)
        };  
    }
}
