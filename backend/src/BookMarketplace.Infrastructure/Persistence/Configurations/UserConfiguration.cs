using BookMarketplace.Domain.Entities;
using BookMarketplace.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookMarketplace.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(x => x.PhoneNumber)
            .IsUnique();

        // Optional. PostgreSQL allows many NULLs in a unique index.
        builder.Property(x => x.Email)
            .HasMaxLength(254);

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.Role)
            .HasConversion<int>()
            .HasDefaultValue(UserRole.Customer);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.FailedLoginCount)
            .HasDefaultValue(0);
    }
}