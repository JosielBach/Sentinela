using Bogus;
using Sentinela.Domain.Entities;
using Sentinela.Domain.Enums;

namespace CommonTestUtilities.Entities;

public class AssetBuilder
{
    public static IList<Asset> Collection(int count = 3)
    {
        return new Faker<Asset>()
            .RuleFor(asset => asset.Hostname, f => f.Internet.DomainName())
            .RuleFor(asset => asset.Type, f => f.PickRandom<AssetType>())
            .RuleFor(asset => asset.LastSeenAt, f => f.Date.Recent())
            .RuleFor(asset => asset.ApiKeyHash, f => f.Random.Hash())
            .Generate(count);
    }
}
