using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Sentinela.Domain.Entities;
using Sentinela.Domain.Identity;
using Sentinela.Domain.Security.Tokens;
using Sentinela.Infrastructure.DataAccess;

namespace Sentinela.Infrastructure.Identity;

internal sealed class LoggedUser : ILoggedUser
{
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly SentinelaDbContext _dbContext;
    public LoggedUser(IAccessTokenProvider accessTokenProvider, SentinelaDbContext dbContext)
    {
        _accessTokenProvider = accessTokenProvider;
        _dbContext = dbContext;
    }
    public async Task<User> Get()
    {
        var userId = GetUserId();

        return await _dbContext.Users.AsNoTracking().FirstAsync(user => user.Active && user.Id == userId);
    }

    public Guid GetUserId()
    {
        var accessToken = _accessTokenProvider.GetToken();

        var handler = new JsonWebTokenHandler();

        var jsonWebToken = handler.ReadJsonWebToken(accessToken);

        var subjectClaim = jsonWebToken.Claims.First(c => c.Type.Equals(JwtRegisteredClaimNames.Sub));

        return Guid.Parse(subjectClaim.Value);
    }
}
