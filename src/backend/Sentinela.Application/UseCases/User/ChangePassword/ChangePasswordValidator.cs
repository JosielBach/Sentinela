using FluentValidation;
using Sentinela.Application.UseCases.Shared.Validators;
using Sentinela.Communication.Requests;

namespace Sentinela.Application.UseCases.User.ChangePassword;

public class ChangePasswordValidator : AbstractValidator<RequestChangePasswordJson>
{
    public ChangePasswordValidator()
    {
        RuleFor(request => request.NewPassword).Password();
    }
}
