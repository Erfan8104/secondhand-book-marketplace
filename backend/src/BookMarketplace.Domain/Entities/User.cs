using BookMarketplace.Domain.Common;

namespace BookMarketplace.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public bool IsSeller { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<Book> Books { get; set; } = new List<Book>();

    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}