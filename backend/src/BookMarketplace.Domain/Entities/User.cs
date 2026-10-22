using BookMarketplace.Domain.Common;
using BookMarketplace.Domain.Enums;

namespace BookMarketplace.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    // Login identifier. Always stored in E.164 format, e.g. +989121234567
    public string PhoneNumber { get; set; } = string.Empty;

    // Optional. Null when the user did not provide one.
    public string? Email { get; set; }

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Customer;

    public bool IsActive { get; set; } = true;

    // Brute-force protection
    public int FailedLoginCount { get; set; }

    public DateTime? LockoutEndUtc { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();

    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();

    public ICollection<RefreshToken> RefreshTokens { get; set; } =
        new List<RefreshToken>();
}