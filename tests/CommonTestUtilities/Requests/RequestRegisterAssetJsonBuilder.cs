using Bogus;
using Sentinela.Communication.Requests;
using Sentinela.Communication.Enums;

namespace CommonTestUtilities.Requests;

public class RequestRegisterAssetJsonBuilder
{
    public static RequestRegisterAssetJson Build()
    {
        return new Faker<RequestRegisterAssetJson>().RuleFor(request => request.Hostname, f => f.Internet.DomainName())
            .RuleFor(request => request.Type, f => f.PickRandom(AssetType.Workstation, AssetType.Server));
        
    }
}
