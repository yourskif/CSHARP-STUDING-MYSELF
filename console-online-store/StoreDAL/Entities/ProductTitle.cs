namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Shared title/label of a product (SKU family), linked to a category.
/// </summary>
[Table("product_titles")]
public class ProductTitle : BaseEntity
{
    public ProductTitle()
        : base()
    {
    }

    public ProductTitle(int id, string title, int categoryId)
        : base(id)
    {
        this.Title = title;
        this.CategoryId = categoryId;
    }

    [Column("title")]
    public string? Title { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual IList<Product> Products { get; set; } = new List<Product>();
}
