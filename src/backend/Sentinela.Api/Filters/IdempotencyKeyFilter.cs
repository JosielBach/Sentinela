using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sentinela.Communication.Responses;
using Sentinela.Domain.Extensions;
using Sentinela.Exception;

namespace Sentinela.Api.Filters;

public class IdempotencyKeyFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var idempotencyKey = context.HttpContext.Request.Headers["Idempotency-Key"].ToString();

        if (idempotencyKey.IsEmpty())
        {
            context.Result = new BadRequestObjectResult(new ResponseErrorJson(ResourceMessagesException.VALIDATION_IDEMPOTENCY_KEY_REQUIRED));
            return;
        }

        await next();
    }
}
