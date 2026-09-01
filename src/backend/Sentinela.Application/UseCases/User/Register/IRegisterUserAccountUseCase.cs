using Sentinela.Communication.Requests;
using Sentinela.Communication.Responses;

namespace Sentinela.Application.UseCases.User.Register;

public interface IRegisterUserAccountUseCase
{
    Task<ResponseRegisterUserJson> Execute(RequestRegisterUserAccountJson request);
}
