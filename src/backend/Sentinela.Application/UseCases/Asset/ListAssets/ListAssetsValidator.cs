using FluentValidation;
using Sentinela.Communication.Requests;
using Sentinela.Exception;


namespace Sentinela.Application.UseCases.Asset.ListAssets;

public class ListAssetsValidator : AbstractValidator<RequestListAssetsJson>
{
    public ListAssetsValidator()
    {
        RuleFor(request => request.Page).GreaterThanOrEqualTo(1).WithMessage(ResourceMessagesException.VALIDATION_PAGE_INVALID);
        RuleFor(request => request.PageSize).InclusiveBetween(1, 100).WithMessage(ResourceMessagesException.VALIDATION_PAGE_SIZE_INVALID);
    }
}
