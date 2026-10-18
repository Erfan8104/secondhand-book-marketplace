using BookMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMarketplace.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders", table =>
        {
            table.HasCheckConstraint(
                "CK_orders_amounts_nonnegative",
                "\"Subtotal\" >= 0 AND \"ShippingCost\" >= 0 " +
                "AND \"CommissionAmount\" >= 0 AND \"TotalAmount\" >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.OrderNumber)
            .IsUnique();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.PaymentStatus)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Subtotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.ShippingCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.CommissionAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.RecipientName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.RecipientPhone)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.ShippingProvince)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ShippingCity)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.ShippingPostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ShippingFullAddress)
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasIndex(x => new { x.BuyerId, x.CreatedAtUtc });

        builder.HasIndex(x => new { x.SellerId, x.CreatedAtUtc });

        builder.HasOne(x => x.Buyer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.BuyerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Seller)
            .WithMany()
            .HasForeignKey(x => x.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Items)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}