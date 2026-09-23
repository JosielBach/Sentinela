using Sentinela.Communication.Requests;
using Sentinela.Domain.Identity;
using Sentinela.Domain.Repositories;
using Sentinela.Domain.Repositories.User;
using Sentinela.Exception;
using Sentinela.Exception.ExceptionBase;

namespace Sentinela.Application.UseCases.User.Update;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateUserUseCase(ILoggedUser loggedUser, IUserReadOnlyRepository userReadOnlyRepository,
        IUserUpdateOnlyRepository userUpdateOnlyRepository, IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _userReadOnlyRepository = userReadOnlyRepository;
        _userUpdateOnlyRepository = userUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task Execute(RequestUpdateUserJson request)
    {
        var loggedUser = await _loggedUser.Get();

        await Validate(request, loggedUser);

        loggedUser.Name = request.Name;
        loggedUser.Email = request.Email;

        _userUpdateOnlyRepository.UpdateProfile(loggedUser);
        await _unitOfWork.Commit();
    }

    private async Task Validate(RequestUpdateUserJson request, Domain.Entities.User loggedUser)
    {
        var validator = new UpdateUserValidator();

        var result = validator.Validate(request);

        if(loggedUser.Email.Equals(request.Email) == false)
        {
            var userExist = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);
            if (userExist)
                result.Errors.Add(new FluentValidation.Results.ValidationFailure("Email", ResourceMessagesException.VALIDATION_EMAIL_ALREDY_EXISTS));
        }

        if (result.IsValid == false)
        {
            var errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages); 
        }
    }
}
