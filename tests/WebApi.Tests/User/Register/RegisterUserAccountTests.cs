using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Sentinela.Domain.Extensions;
using Sentinela.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace WebApi.Tests.User.Register;

public class RegisterUserAccountTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/users";
    public RegisterUserAccountTests(SentinelaApplicationFactory factory) : base(factory)
    {
 
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build();

        var response = await Post(REQUEST_URI, request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString().ShouldBe(request.Name);
        responseData.RootElement.GetProperty("tokens").GetProperty("accessToken").GetString().ShouldNotBeEmpty();
        responseData.RootElement.GetProperty("tokens").GetProperty("refreshToken").GetString().ShouldBeEmpty();

        var userexist = await DbContext.Users.AnyAsync(user => user.Active && user.Name.Equals(request.Name) && user.Email.Equals(request.Email));
        
        userexist.ShouldBeTrue();
    }

    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Validate_ShouldBeAnErrorResponse_WhenNameIsEmpty(string culture)
    {
        var request = RequestRegisterUserAccountJsonBuilder.Build();
        request.Name = string.Empty;

        var response = await Post(REQUEST_URI, request, culture: culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var errorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_NAME_REQUIRED", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString().IsNotEmpty()! && error.GetString()!.Equals(errorMessage));
        });

        var userexist = await DbContext.Users.AnyAsync(user => user.Active && user.Name.Equals(request.Name) && user.Email.Equals(request.Email));

        userexist.ShouldBeFalse();
    }
}
