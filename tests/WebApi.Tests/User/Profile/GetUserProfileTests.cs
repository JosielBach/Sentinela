using Sentinela.Domain.Entities;
using Shouldly;
using System.Net;
using System.Text.Json;
using WebApi.Tests.Resources;

namespace WebApi.Tests.User.Profile;

public class GetUserProfileTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/api/v1/users";
    private readonly UserIdentityManager _user01;

    public GetUserProfileTests(SentinelaApplicationFactory factory) : base(factory)
    {
        _user01 = factory.User01;
    }

    [Fact]
    public async Task Success()
    {
        var response = await Get(REQUEST_URI, accessToken: _user01.GetAccessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldBe(_user01.GetName());
        responseData.RootElement.GetProperty("email").GetString().ShouldBe(_user01.GetEmail());
    }
}
