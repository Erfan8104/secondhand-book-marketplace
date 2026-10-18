using BookMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMarketplace.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration
    : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("order_items", table =>
        {
            table.HasCheckConstraint(
                "CK_order_items_quantity_positive",
                "\"Quantity\" > 0");

            table.HasCheckConstraint(
                "CK_order_items_prices_nonnegative",
                "\"UnitPrice\" >= 0 AND \"LineTotal\" >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BookTitle)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.LineTotal)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.OrderId);

        builder.HasIndex(x => x.BookId);

        builder.HasOne(x => x.Book)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}