using CommonTestUtilities.Requests;
using Shouldly;
using System.Net;

namespace WebApi.Tests.Asset.Register;

public class RegisterAssetTokenTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/api/v1/assets";
    private readonly string _tokenUserNotExistDatabase;

    public RegisterAssetTokenTests(SentinelaApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATA_BASE;
    }

    [Fact]
    public async Task Error_Token_Invalid()
    {
        var request = RequestRegisterAssetJsonBuilder.Build();

        var response = await Post(REQUEST_URI, request, accessToken: "tokenInvalido.");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Without_Token()
    {
        var request = RequestRegisterAssetJsonBuilder.Build();

        var response = await Post(REQUEST_URI, request, accessToken: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Error_Token_Whith_User_Not_Found()
    {
        var request = RequestRegisterAssetJsonBuilder.Build();

        var response = await Post(REQUEST_URI, request, _tokenUserNotExistDatabase);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
