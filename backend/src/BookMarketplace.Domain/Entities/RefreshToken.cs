using BookMarketplace.Domain.Common;

namespace BookMarketplace.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    // SHA-256 hash (hex) of the raw token. The raw token is never stored.
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public DateTime? RevokedAtUtc { get; set; } 

    // Set when this token was rotated into a new one.
    public string? ReplacedByTokenHash { get; set; }

    public string? CreatedByIp { get; set; }

    public bool IsRevoked() => RevokedAtUtc.HasValue;

    public bool IsExpired(DateTime utcNow) => utcNow >= ExpiresAtUtc;

    public bool IsActive(DateTime utcNow) =>
        !IsRevoked() && !IsExpired(utcNow);
}