using Sentinela.Communication.Requests;
using Shouldly;
using System.Net;

namespace WebApi.Tests.User.Update;

public class UpdateUserTokenTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/users/profile";
    private readonly string _tokenUserNotExistDatabase;

    public UpdateUserTokenTests(SentinelaApplicationFactory factory) : base(factory)
    {
        _tokenUserNotExistDatabase = factory.TOKEN_USER_NOT_FOUND_IN_DATA_BASE;
    }

    public async Task Error_Token_Invalid()
    {
        var request = new RequestChangePasswordJson();

        var response = await Put(REQUEST_URI, request, accessToken: "tokenInvalido.");

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    public async Task Error_Without_Token()
    {
        var request = new RequestChangePasswordJson();

        var response = await Put(REQUEST_URI, request, accessToken: string.Empty);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
    public async Task Error_Token_Whith_User_Not_Found()
    {
        var request = new RequestChangePasswordJson();

        var response = await Put(REQUEST_URI, request, _tokenUserNotExistDatabase);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
