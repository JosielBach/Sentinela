using Microsoft.EntityFrameworkCore;
using Sentinela.Domain.Entities;

namespace Sentinela.Infrastructure.DataAccess;

internal class SentinelaDbContext : DbContext
{
    public SentinelaDbContext(DbContextOptions dbContext) : base(dbContext) { }
    
    public DbSet<User> Users {  get; set; }
}
