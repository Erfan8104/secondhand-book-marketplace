using BookMarketplace.Domain.Common;

namespace BookMarketplace.Domain.Entities;

public class OrderItem : BaseEntity
{
    // Snapshot: product details and price at purchase time.
    public string BookTitle { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }

    public Guid OrderId { get; set; }

    public Order Order { get; set; } = null!;

    public Guid BookId { get; set; }

    public Book Book { get; set; } = null!;
}