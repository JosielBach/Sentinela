using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Sentinela.Domain.Extensions;
using Sentinela.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using WebApi.Tests.Resources;

namespace WebApi.Tests.User.Update;

public class UpdateUserTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/users/profile";
    private readonly UserIdentityManager _user01;
    public UpdateUserTests(SentinelaApplicationFactory factory) : base(factory)
    {
        _user01 = factory.User01;
    }
    [Fact]
    public async Task Success()
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = _user01.GetName();
        request.Email = _user01.GetEmail();

        var response = await Put(REQUEST_URI, request, accessToken: _user01.GetAccessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var userexist = await DbContext.Users.AnyAsync(user => user.Active && user.Name.Equals(request.Name) && user.Email.Equals(request.Email));

        userexist.ShouldBeTrue();
    }
    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Error_EmptyName(string culture)
    {
        var request = RequestUpdateUserJsonBuilder.Build();
        request.Name = string.Empty;

        var response = await Put(REQUEST_URI, request, accessToken: _user01.GetAccessToken(), culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();

        var expectedMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_NAME_REQUIRED", new CultureInfo(culture));
        errors.ShouldSatisfyAllConditions(error =>
        {
            errors.Count().ShouldBe(1);
            errors.ShouldContain(error => error.GetString().IsNotEmpty() && error.GetString()!.Equals(expectedMessage));
        });
    }
}
