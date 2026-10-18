using BookMarketplace.Domain.Common;

namespace BookMarketplace.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public Guid? ParentCategoryId { get; set; }

    public Category? ParentCategory { get; set; }

    public ICollection<Category> Subcategories { get; set; }
        = new List<Category>();

    public ICollection<Book> Books { get; set; } = new List<Book>();
}