using BookMarketplace.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace BookMarketplace.Infrastructure.Persistence;

public sealed class MarketplaceDbContext(
    DbContextOptions<MarketplaceDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MarketplaceDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasKey(nameof(BaseEntity.Id));
            }
        }
    }
}