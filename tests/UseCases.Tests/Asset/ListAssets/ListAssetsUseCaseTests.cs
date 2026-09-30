using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Sentinela.Application.UseCases.Asset.ListAssets;
using Sentinela.Exception;
using Sentinela.Exception.ExceptionBase;
using Shouldly;

namespace UseCases.Tests.Asset.ListAssets;

public class ListAssetsUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var request = RequestListAssetsJsonBuilder.Build();
        var assets = AssetBuilder.Collection();
        var totalCount = assets.Count + 20;

        var useCase = CreateUseCase(request.Page, request.PageSize, assets, totalCount);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(totalCount);
        result.Assets.Count.ShouldBe(assets.Count);
        result.Assets.Select(asset => asset.Id).ShouldBe(assets.Select(asset => asset.Id));
        result.Assets.Select(asset => asset.Hostname).ShouldBe(assets.Select(asset => asset.Hostname));
        result.Assets.Select(asset => asset.Type.ToString()).ShouldBe(assets.Select(asset => asset.Type.ToString()));
        result.Assets.Select(asset => asset.LastSeenAt).ShouldBe(assets.Select(asset => asset.LastSeenAt));
    }

    [Fact]
    public async Task Success_WithoutAssets()
    {
        var request = RequestListAssetsJsonBuilder.Build();

        var useCase = CreateUseCase(request.Page, request.PageSize, [], 0);

        var result = await useCase.Execute(request);

        result.ShouldNotBeNull();
        result.Assets.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenPageIsInvalid()
    {
        var request = RequestListAssetsJsonBuilder.Build();
        request.Page = 0;

        var useCase = CreateUseCase(request.Page, request.PageSize, AssetBuilder.Collection(), 3);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_PAGE_INVALID);
        });
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenPageSizeIsInvalid()
    {
        var request = RequestListAssetsJsonBuilder.Build();
        request.PageSize = 0;

        var useCase = CreateUseCase(request.Page, request.PageSize, AssetBuilder.Collection(), 3);

        var exception = await useCase.Execute(request).ShouldThrowAsync<ErrorOnValidationException>();
        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_PAGE_SIZE_INVALID);
        });
    }

    private static ListAssetsUseCase CreateUseCase(int page, int pageSize, IList<Sentinela.Domain.Entities.Asset> assets, int totalCount)
    {
        var assetReadOnlyRepository = new IAssetReadOnlyRepositoryBuilder();
        assetReadOnlyRepository.GetAll(page, pageSize, assets);
        assetReadOnlyRepository.CountAll(totalCount);

        return new ListAssetsUseCase(assetReadOnlyRepository.Build());
    }
}
