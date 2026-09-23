using Sentinela.Domain.Entities;

namespace Sentinela.Domain.Security.Tokens;

public interface IAccessTokenGenerator
{
    string Generate(User user);
}
