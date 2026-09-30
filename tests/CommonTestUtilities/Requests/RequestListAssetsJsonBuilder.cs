using Bogus;
using Sentinela.Communication.Requests;

namespace CommonTestUtilities.Requests;

public class RequestListAssetsJsonBuilder
{
    public static RequestListAssetsJson Build()
    {
        return new Faker<RequestListAssetsJson>()
            .RuleFor(request => request.Page, f => f.Random.Int(1, 10))
            .RuleFor(request => request.PageSize, f => f.Random.Int(1, 100));
    }
}
