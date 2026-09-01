namespace Sentinela.Exception.ExceptionBase;

public class ErrorOnValidationException : SentinelaException
{
    private readonly List<string> _errors;
    public ErrorOnValidationException(List<string> errorMessages)
    {
        _errors = errorMessages;
    }
    public List<string> GetErrorMessages() => _errors;
}
