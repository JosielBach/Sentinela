using Sentinela.Communication.Enums;

namespace Sentinela.Communication.Requests;

public class RequestRegisterAssetJson
{
    public string Hostname { get; set; } = string.Empty;
    public AssetType Type { get; set; }
}
