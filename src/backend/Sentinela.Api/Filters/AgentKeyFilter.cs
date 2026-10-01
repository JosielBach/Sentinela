using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sentinela.Communication.Responses;
using Sentinela.Domain.Extensions;
using Sentinela.Domain.Repositories.Asset;
using Sentinela.Domain.Security.ApiKeyHashing;
using Sentinela.Exception;

namespace Sentinela.Api.Filters;

public class AgentKeyFilter : IAsyncAuthorizationFilter
{
    private readonly IApiKeyHasher _apiKeyHasher;
    private readonly IAssetReadOnlyRepository _assetReadOnlyRepository;
    public AgentKeyFilter(IApiKeyHasher apiKeyHasher, IAssetReadOnlyRepository assetReadOnlyRepository)
    {
        _apiKeyHasher = apiKeyHasher;
        _assetReadOnlyRepository = assetReadOnlyRepository;
    }
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.Request.Headers["X-Agent-Key"].ToString();

        var asset = apiKey.IsEmpty() 
            ? null : await _assetReadOnlyRepository.GetByApiKeyHash(_apiKeyHasher.HashApiKey(apiKey));

        if (asset is null || asset.Active == false)
        {
            context.Result = new UnauthorizedObjectResult(new ResponseErrorJson(ResourceMessagesException.VALIDATION_RESOURCE_ACCESS_DENIED));
            return;
        }

        context.HttpContext.Items["AssetId"] = asset.Id;
    }
}
