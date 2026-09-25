using Sentinela.Domain.Enums;

namespace Sentinela.Domain.Entities;

public class Asset
{
    public Guid Id { get; private set; } = Guid.CreateVersion7();
    public bool Active { get; set; } = true;
    public string Hostname { get; set; } = string.Empty;
    public AssetType Type { get; set; }
    public string ApiKeyHash { get; set; } = string.Empty;
    public DateTime? LastSeenAt { get; set; }

    public bool IsOffline(DateTime now, TimeSpan threshold) =>
        LastSeenAt is null || now - LastSeenAt.Value > threshold;
}
