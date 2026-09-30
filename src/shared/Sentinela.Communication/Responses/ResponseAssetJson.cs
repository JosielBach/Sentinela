using Sentinela.Communication.Enums;

namespace Sentinela.Communication.Responses;

public class ResponseAssetJson
{
    public Guid Id { get; set; }
    public bool Active { get; set; } 
    public string Hostname { get; set; } = string.Empty;
    public AssetType Type { get; set; }
    public DateTime? LastSeenAt { get; set; }
}
