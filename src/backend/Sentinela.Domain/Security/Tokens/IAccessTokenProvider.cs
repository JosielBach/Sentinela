namespace Sentinela.Domain.Security.Tokens;

public interface IAccessTokenProvider
{
    string GetToken();
}
