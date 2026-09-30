using Mapster;
using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;
using Sentinela.Domain.Repositories.Asset;
using Sentinela.Exception.ExceptionBase;

namespace Sentinela.Application.UseCases.Asset.ListAssets;

public class ListAssetsUseCase : IListAssetsUseCase
{
    private readonly IAssetReadOnlyRepository _assetReadOnlyRepository;
    public ListAssetsUseCase(IAssetReadOnlyRepository assetReadOnlyRepository)
    {
        _assetReadOnlyRepository = assetReadOnlyRepository;
    }
    public async Task<ResponseAssetsJson> Execute(RequestListAssetsJson request)
    {
        Validate(request);

        var assets = await _assetReadOnlyRepository.GetAll(request.Page, request.PageSize);
        var totalCount = await _assetReadOnlyRepository.CountAll();

        return new ResponseAssetsJson
        {
            Assets = assets.Adapt<IList<ResponseAssetJson>>(),
            TotalCount = totalCount
        };
    }

    private static void Validate(RequestListAssetsJson request)
    {
        var result = new ListAssetsValidator().Validate(request);

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}
