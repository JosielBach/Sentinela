namespace Sentinela.Communication.Responses;

public class ResponseAssetsJson
{
    public IList<ResponseAssetJson> Assets { get; set; } = [];
    public int TotalCount { get; set; }
}
