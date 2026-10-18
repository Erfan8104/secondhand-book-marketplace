using BookMarketplace.Domain.Common;
using BookMarketplace.Domain.Entities;
namespace BookMarketplace.Domain.Entities;

public class Address : BaseEntity
{
    public string RecipientName { get; set; } = string.Empty;

    public string RecipientPhone { get; set; } = string.Empty;

    public string Province { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string FullAddress { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}