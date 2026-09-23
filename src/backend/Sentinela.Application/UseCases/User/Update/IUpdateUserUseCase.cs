using Sentinela.Communication.Requests;

namespace Sentinela.Application.UseCases.User.Update;

public interface IUpdateUserUseCase
{
    Task Execute(RequestUpdateUserJson request);
}
