using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;

namespace Sentinela.Application.UseCases.Asset.Register;

public interface IRegisterAssetUseCase
{
    Task<ResponseRegisterAssetJson> Execute(RequestRegisterAssetJson request);
}
