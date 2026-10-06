using Microsoft.Extensions.DependencyInjection;
using Sentinela.Domain.Extensions;
using Sentinela.Infrastructure.DataAccess;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebApi.Tests;

public abstract class BaseIntegrationTest : IClassFixture<SentinelaApplicationFactory>, IDisposable
{
    internal readonly SentinelaDbContext DbContext;
    private readonly HttpClient _httpClient;
    private readonly IServiceScope _sope;
    public BaseIntegrationTest(SentinelaApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
        _sope = factory.Services.CreateScope();
        DbContext = _sope.ServiceProvider.GetRequiredService<SentinelaDbContext>();
    }

    protected async Task<HttpResponseMessage> Post(string requestUri, object request, string accessToken = "", string culture = "pt-BR"
        ,IDictionary<string, string>? headers = null)
    {
        ChangeCulture(culture);
        AuthorizeRequest(accessToken);
        AddHeaders(headers);

        return await _httpClient.PostAsJsonAsync(requestUri, request);
    }

    protected async Task<HttpResponseMessage> Put(string requestUri, object request, string accessToken, string culture = "pt-BR")
    {
        ChangeCulture(culture);
        AuthorizeRequest(accessToken);

        return await _httpClient.PutAsJsonAsync(requestUri, request);
    }

    protected async Task<HttpResponseMessage> Get(string requestUri, string accessToken, string culture = "pt-BR")
    {
        ChangeCulture(culture);
        AuthorizeRequest(accessToken);

        return await _httpClient.GetAsync(requestUri);
    }
    private void ChangeCulture(string culture)
    {
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);
    }
    private void AuthorizeRequest(string accessToken)
    {
        if (accessToken.IsNotEmpty())
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public void Dispose()
    {
        _sope?.Dispose();
        DbContext?.Dispose();
    }
    private void AddHeaders(IDictionary<string, string>? headers)
    {
        if (headers is null)
            return;

        foreach (var header in headers)
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
    }
}
