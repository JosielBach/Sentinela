using Shouldly;
using System.Net;

namespace WebApi.Tests.Asset.ListAssets;

public class ListAssetsTokenTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/api/v1/assets";
    private readonly string _tokenUserNotExistDatabase;

    public ListAssetsTokenTests(SentinelaApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATA_BASE;
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var response = await Get(REQUEST_URI, accessToken: "tokenInvalido.");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var response = await Get(REQUEST_URI, accessToken: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_Whith_User_Not_Found()
    {
        var response = await Get(REQUEST_URI, _tokenUserNotExistDatabase);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
