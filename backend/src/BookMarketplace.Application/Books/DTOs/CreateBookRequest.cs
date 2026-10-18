
using System.ComponentModel.DataAnnotations;
using BookMarketplace.Domain.Enums;

namespace BookMarketplace.Application.Books.DTOs;

public sealed class CreateBookRequest
{
    [Required(ErrorMessage = "Book title is required.")]
    [StringLength(200, MinimumLength = 2)]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "Author name is required.")]
    [StringLength(150, MinimumLength = 2)]
    public string Author { get; init; } = string.Empty;

    [StringLength(20)]
    public string? ISBN { get; init; }

    [StringLength(2000)]
    public string? Description { get; init; }

    [Range(
        typeof(decimal),
        "0.01",
        "999999999.99",
        ErrorMessage = "Price must be greater than zero.")]
    public decimal Price { get; init; }

    [EnumDataType(typeof(BookCondition))]
    public BookCondition Condition { get; init; }

    [Range(0, 10000)]
    public int Quantity { get; init; } = 1;

    [Required(ErrorMessage = "Category ID is required.")]
    public Guid? CategoryId { get; init; }
}