using Microsoft.EntityFrameworkCore;
using Sentinela.Domain.Entities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WebApi.Tests")]
namespace Sentinela.Infrastructure.DataAccess;

internal class SentinelaDbContext : DbContext
{
    public SentinelaDbContext(DbContextOptions dbContext) : base(dbContext) { }
    
    public DbSet<User> Users {  get; set; }
}
