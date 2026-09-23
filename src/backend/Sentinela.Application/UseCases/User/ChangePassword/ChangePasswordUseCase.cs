using FluentValidation.Results;
using Sentinela.Communication.Requests;
using Sentinela.Domain.Identity;
using Sentinela.Domain.Repositories.User;
using Sentinela.Domain.Security.PasswordHashing;
using Sentinela.Exception;
using Sentinela.Exception.ExceptionBase;

namespace Sentinela.Application.UseCases.User.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
    public ChangePasswordUseCase(ILoggedUser loggedUser, IPasswordHasher passwordHasher, IUserUpdateOnlyRepository userUpdateOnlyRepository)
    {
        _loggedUser = loggedUser;
        _passwordHasher = passwordHasher;
        _userUpdateOnlyRepository = userUpdateOnlyRepository;
    }

    public async Task Execute(RequestChangePasswordJson request)
    {
        var loggedUser = await _loggedUser.Get();

        Validate(request, loggedUser);

        var hashedPassword = _passwordHasher.HashPassword(request.NewPassword);

        await _userUpdateOnlyRepository.UpdatePassword(loggedUser.Id, hashedPassword);
    }

    private void Validate(RequestChangePasswordJson request, Domain.Entities.User loggedUser)
    {
        var result = new ChangePasswordValidator().Validate(request);

        if (_passwordHasher.VerifyPassword(request.CurrentPassword, loggedUser.Password) == false)
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessagesException.VALIDATION_CURRENT_PASSWORD));

        if (result.IsValid == false)
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).ToList());
    }
}
