using Mapster;
using Sentinela.Communication.Responses;
using Sentinela.Domain.Identity;

namespace Sentinela.Application.UseCases.User.Profile;

public class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly ILoggedUser _loggedUser;
    public GetUserProfileUseCase(ILoggedUser loggedUser)
    {
        _loggedUser = loggedUser;
    }
    public async Task<ResponseUserProfileJson> Execute()
    {
        var loggedUser = await _loggedUser.Get();
        return loggedUser.Adapt<ResponseUserProfileJson>();
    }
}
