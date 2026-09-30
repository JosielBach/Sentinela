using Microsoft.EntityFrameworkCore;
using Sentinela.Domain.Entities;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("WebApi.Tests")]
namespace Sentinela.Infrastructure.DataAccess;

internal class SentinelaDbContext : DbContext
{
    public SentinelaDbContext(DbContextOptions dbContext) : base(dbContext) { }
    
    public DbSet<User> Users {  get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Sample> Samples { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(user => user.Email).HasMaxLength(100);
            entity.HasIndex(user => user.Email).IsUnique();
        });

        modelBuilder.Entity<Asset>(entity =>
        {
            entity.Property(asset => asset.Hostname).HasMaxLength(253);
            entity.Property(asset => asset.ApiKeyHash).HasMaxLength(64);
            entity.HasIndex(asset => asset.ApiKeyHash).IsUnique();
        });

        modelBuilder.Entity<Sample>(entity =>
        {
            entity.HasIndex(sample => new { sample.AssetId, sample.Metric, sample.CollectedAt })
          .IsDescending(false, false, true);

            entity.HasOne<Asset>()
                  .WithMany()
                  .HasForeignKey(sample => sample.AssetId);
        });
    }
}
