using Sentinela.Communication.Responses;

namespace Sentinela.Application.UseCases.User.Profile;

public interface IGetUserProfileUseCase
{
    Task<ResponseUserProfileJson> Execute();
}
