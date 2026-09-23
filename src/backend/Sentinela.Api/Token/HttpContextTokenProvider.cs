using Sentinela.Domain.Security.Tokens;

namespace Sentinela.Api.Token;

internal sealed class HttpContextTokenProvider : IAccessTokenProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public HttpContextTokenProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string GetToken()
    {
        var accessToken = _httpContextAccessor.HttpContext!.Request.Headers.Authorization.ToString();
        

        return accessToken["Bearer ".Length..];
    }
}
