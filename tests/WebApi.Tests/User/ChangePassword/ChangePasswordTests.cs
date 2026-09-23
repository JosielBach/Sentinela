using CommonTestUtilities.Requests;
using Docker.DotNet.Models;
using Sentinela.Communication.Requests;
using Sentinela.Domain.Extensions;
using Sentinela.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.Resources;

namespace WebApi.Tests.User.ChangePassword;

public class ChangePasswordTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "users/password";
    private readonly UserIdentityManager _user01;

    public ChangePasswordTests(SentinelaApplicationFactory factory) : base(factory)
    {
        _user01 = factory.User01;
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestChangePasswordJsonBuilder.Builder();
        request.CurrentPassword = _user01.GetPassword();

        var response = await Put(REQUEST_URI, request, accessToken: _user01.GetAccessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Error_NewPassword_Empty(string culture)
    {
        var request = new RequestChangePasswordJson
        {
            CurrentPassword = _user01.GetPassword(),
            NewPassword = string.Empty
        };

        var response = await Put(REQUEST_URI, request, accessToken: _user01.GetAccessToken(), culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_PASSWORD_REQUIRED", new CultureInfo(culture));
        errors.ShouldSatisfyAllConditions(error =>
        {
            errors.Count().ShouldBe(1);
            errors.ShouldContain(error => error.GetString().IsNotEmpty() && error.GetString()!.Equals(expectedMessage));
        });
    }
}
