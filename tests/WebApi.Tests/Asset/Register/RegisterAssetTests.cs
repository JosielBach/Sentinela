using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Sentinela.Domain.Extensions;
using Sentinela.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Asset.Register;

public class RegisterAssetTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/api/v1/assets";
    private readonly UserIdentityManager _user01;
    public RegisterAssetTests(SentinelaApplicationFactory factory) : base(factory)
    {
        _user01 = factory.User01;
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestRegisterAssetJsonBuilder.Build();

        var response = await Post(REQUEST_URI, request, accessToken: _user01.GetAccessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var id = responseData.RootElement.GetProperty("id").GetGuid();
        responseData.RootElement.GetProperty("apiKey").GetString().ShouldNotBeNullOrEmpty();

        var assetExist = await DbContext.Assets.AnyAsync(asset => asset.Active && asset.Id == id && asset.Hostname == request.Hostname);

        assetExist.ShouldBeTrue();
    }

    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Validate_ShouldBeAnErrorResponse_WhenHostnameIsEmpty(string culture)
    {
        var request = RequestRegisterAssetJsonBuilder.Build();
        request.Hostname = string.Empty;

        var response = await Post(REQUEST_URI, request, accessToken: _user01.GetAccessToken(), culture: culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var errorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_HOSTNAME_REQUIRED", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString().IsNotEmpty()! && error.GetString()!.Equals(errorMessage));
        });

        var assetExist = await DbContext.Assets.AnyAsync(asset => asset.Active && asset.Hostname == request.Hostname);

        assetExist.ShouldBeFalse();
    }
}
