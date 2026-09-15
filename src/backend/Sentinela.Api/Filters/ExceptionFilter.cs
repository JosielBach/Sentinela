using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sentinela.Communication.Responses;
using Sentinela.Exception;
using Sentinela.Exception.ExceptionBase;

namespace Sentinela.Api.Filters;

public class ExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is SentinelaException sentinelaException)
        {
            context.HttpContext.Response.StatusCode = (int)sentinelaException.GetStatusCode();

            context.Result = new ObjectResult(new ResponseErrorJson(sentinelaException.GetErrorMessages()));
        }
        else
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            context.Result = new ObjectResult(new ResponseErrorJson(ResourceMessagesException.UNKNOWN_ERROR));
        }
    }
}
