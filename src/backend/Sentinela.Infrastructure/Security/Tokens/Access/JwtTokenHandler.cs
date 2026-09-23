using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Sentinela.Domain.Entities;
using Sentinela.Domain.Security.Tokens;
using System.Security.Claims;
using System.Text;

namespace Sentinela.Infrastructure.Security.Tokens.Access;

internal sealed class JwtTokenHandler : IAccessTokenGenerator
{
    private readonly uint _expirationTime;
    private readonly string _signingKey;
    public JwtTokenHandler(uint expirationTime, string signingKey)
    {
        _expirationTime = expirationTime;
        _signingKey = signingKey;
    }
    public string Generate(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddMinutes(_expirationTime),
            SigningCredentials = new SigningCredentials(Credentials(), SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var handler = new JsonWebTokenHandler();

        return handler.CreateToken(tokenDescriptor);
    }

    private SymmetricSecurityKey Credentials()
    {
        var keyBytes = Encoding.UTF8.GetBytes(_signingKey);
        return new SymmetricSecurityKey(keyBytes);
    }
}
