using BookMarketplace.Domain.Common;
using BookMarketplace.Domain.Enums;
using BookMarketplace.Domain.Entities;

namespace BookMarketplace.Domain.Entities;

public class Book : BaseEntity
{
    public string Title { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public string? ISBN { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public BookCondition Condition { get; set; }

    public int Quantity { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public Guid SellerId { get; set; }

    public User Seller { get; set; } = null!;

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; }
        = new List<OrderItem>();
}