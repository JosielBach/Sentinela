using Microsoft.EntityFrameworkCore;
using Sentinela.Domain.Entities;
using Sentinela.Domain.Repositories.User;

namespace Sentinela.Infrastructure.DataAccess.Repositories;

internal sealed class UserRepository : IUserWriteOnlyRepository, IUserReadOnlyRepository, IUserUpdateOnlyRepository
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

    public async Task<bool> ExistActiveUserWithId(Guid userId)
    {
        return await _dbContext.Users.AnyAsync(user => user.Active && user.Id == userId);
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(user => user.Active && user.Email.Equals(email));
    }

    public async Task UpdatePassword(Guid userId, string passwordHash)
    {
        await _dbContext.Users.Where(user => user.Id == userId).ExecuteUpdateAsync(set => set.SetProperty(user => user.Password, passwordHash));
    }

    public void UpdateProfile(User user)
    {
        _dbContext.Users.Attach(user);

        _dbContext.Entry(user).Property(user => user.Name).IsModified = true;
        _dbContext.Entry(user).Property(user => user.Email).IsModified = true;
    }

}