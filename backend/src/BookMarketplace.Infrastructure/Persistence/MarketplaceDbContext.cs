using BookMarketplace.Domain.Common;
using BookMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookMarketplace.Infrastructure.Persistence;

public sealed class MarketplaceDbContext(
    DbContextOptions<MarketplaceDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Book> Books => Set<Book>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Address> Addresses => Set<Address>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

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

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.CreatedAtUtc))
                    .IsRequired();
            }
        }
    }
}