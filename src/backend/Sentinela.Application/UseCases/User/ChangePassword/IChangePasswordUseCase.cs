using Sentinela.Communication.Requests;

namespace Sentinela.Application.UseCases.User.ChangePassword;

public interface IChangePasswordUseCase
{
    Task Execute(RequestChangePasswordJson request);
}
