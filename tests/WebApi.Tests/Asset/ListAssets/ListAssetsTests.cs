using CommonTestUtilities.Entities;
using Microsoft.EntityFrameworkCore;
using Sentinela.Domain.Extensions;
using Sentinela.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Asset.ListAssets;

public class ListAssetsTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/api/v1/assets";
    private readonly UserIdentityManager _user01;
    public ListAssetsTests(SentinelaApplicationFactory factory) : base(factory)
    {
        _user01 = factory.User01;
    }

    [Fact]
    public async Task Success()
    {
        var assets = AssetBuilder.Collection();
        await DbContext.Assets.AddRangeAsync(assets);
        await DbContext.SaveChangesAsync();

        var response = await Get($"{REQUEST_URI}?page=1&pageSize=100", accessToken: _user01.GetAccessToken());

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var totalCount = await DbContext.Assets.CountAsync();
        responseData.RootElement.GetProperty("totalCount").GetInt32().ShouldBe(totalCount);

        var responseAssets = responseData.RootElement.GetProperty("assets").EnumerateArray().ToList();
        responseAssets.Count.ShouldBe(totalCount);

        foreach (var asset in assets)
        {
            responseAssets.ShouldContain(responseAsset => responseAsset.GetProperty("id").GetGuid() == asset.Id
                && responseAsset.GetProperty("hostname").GetString() == asset.Hostname);
        }

        foreach (var responseAsset in responseAssets)
        {
            responseAsset.TryGetProperty("apiKeyHash", out _).ShouldBeFalse();
        }
    }

    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Validate_ShouldBeAnErrorResponse_WhenPageIsInvalid(string culture)
    {
        var response = await Get($"{REQUEST_URI}?page=0&pageSize=10", accessToken: _user01.GetAccessToken(), culture: culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var errorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_PAGE_INVALID", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString().IsNotEmpty()! && error.GetString()!.Equals(errorMessage));
        });
    }

    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Validate_ShouldBeAnErrorResponse_WhenPageSizeIsInvalid(string culture)
    {
        var response = await Get($"{REQUEST_URI}?page=1&pageSize=101", accessToken: _user01.GetAccessToken(), culture: culture);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var errorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_PAGE_SIZE_INVALID", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString().IsNotEmpty()! && error.GetString()!.Equals(errorMessage));
        });
    }
}
