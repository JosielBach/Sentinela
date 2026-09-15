using System.Net;

namespace Sentinela.Exception.ExceptionBase;

public abstract class SentinelaException : System.Exception
{
    public abstract HttpStatusCode GetStatusCode();
    public abstract List<string> GetErrorMessages();
}
