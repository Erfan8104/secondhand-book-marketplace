using BookMarketplace.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMarketplace.Infrastructure.Persistence.Configurations;

public sealed class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books", table =>
        {
            table.HasCheckConstraint(
                "CK_books_price_positive",
                "\"Price\" > 0");

            table.HasCheckConstraint(
                "CK_books_quantity_nonnegative",
                "\"Quantity\" >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Author)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.ISBN)
            .HasMaxLength(20);

        builder.HasIndex(x => x.ISBN);

        builder.Property(x => x.Description)
            .HasMaxLength(5000);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Condition)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasDefaultValue(1);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(x => new { x.CategoryId, x.IsActive });

        builder.HasIndex(x => new { x.SellerId, x.IsActive });

        builder.HasOne(x => x.Seller)
            .WithMany(x => x.Books)
            .HasForeignKey(x => x.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Books)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}