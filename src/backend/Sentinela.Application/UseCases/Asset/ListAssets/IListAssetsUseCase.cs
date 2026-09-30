using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;

namespace Sentinela.Application.UseCases.Asset.ListAssets;

public interface IListAssetsUseCase
{
    Task<ResponseAssetsJson> Execute(RequestListAssetsJson request);
}