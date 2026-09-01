using Microsoft.EntityFrameworkCore;
using Sentinela.Domain.Entities;
using Sentinela.Domain.Repositories.User;

namespace Sentinela.Infrastructure.DataAccess.Repositories;

internal sealed class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository
{
    private readonly SentinelaDbContext _dbContext;
    public UserRepository(SentinelaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(User user) => await _dbContext.Users.AddAsync(user);

    public async Task<bool> ExistActiveUserWithEmail(string email)
    {
        return await _dbContext.Users.AnyAsync(user => user.Active && user.Email.Equals(email));
    }
}