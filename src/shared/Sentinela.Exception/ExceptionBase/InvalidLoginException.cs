using System.Net;

namespace Sentinela.Exception.ExceptionBase;

public class InvalidLoginException : SentinelaException
{
    public override List<string> GetErrorMessages() => [ResourceMessagesException.VALIDATION_LOGIN_INVALID];

    public override HttpStatusCode GetStatusCode() => HttpStatusCode.Unauthorized;
}
