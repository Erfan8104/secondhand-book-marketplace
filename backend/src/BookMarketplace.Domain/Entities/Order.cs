using BookMarketplace.Domain.Common;
using BookMarketplace.Domain.Enums;
using BookMarketplace.Domain.Entities;

namespace BookMarketplace.Domain.Entities;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public PaymentStatus PaymentStatus { get; set; }
        = PaymentStatus.Pending;

    public decimal Subtotal { get; set; }

    public decimal ShippingCost { get; set; }

    public decimal CommissionAmount { get; set; }

    public decimal TotalAmount { get; set; }

    // Snapshot of the shipping address at checkout.
    public string RecipientName { get; set; } = string.Empty;

    public string RecipientPhone { get; set; } = string.Empty;

    public string ShippingProvince { get; set; } = string.Empty;

    public string ShippingCity { get; set; } = string.Empty;

    public string ShippingPostalCode { get; set; } = string.Empty;

    public string ShippingFullAddress { get; set; } = string.Empty;

    public DateTime? ShippedAtUtc { get; set; }

    public DateTime? DeliveredAtUtc { get; set; }

    public Guid BuyerId { get; set; }

    public User Buyer { get; set; } = null!;

    // Every order belongs to exactly one seller.
    public Guid SellerId { get; set; }

    public User Seller { get; set; } = null!;

    public ICollection<OrderItem> Items { get; set; }
        = new List<OrderItem>();
}