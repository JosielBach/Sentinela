using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;

namespace Sentinela.Application.UseCases.Login.WithEmailAndPassword;

public interface ILoginWithEmailAndPasswordUseCase
{
    Task<ResponseRegisterUserJson> Execute(RequestLoginJson request);
}
