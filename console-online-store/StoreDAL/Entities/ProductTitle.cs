// Path: console-online-store/StoreDAL/Entities/ProductTitle.cs
namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Shared title/label of a product (SKU family), linked to a category.
/// Table structure matches TZ diagram exactly.
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

    /// <summary>
    /// Gets or sets the product title/name.
    /// Database column: product_title (per TZ diagram)
    /// </summary>
    [Column("product_title")]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the category ID (foreign key).
    /// Database column: category_id
    /// </summary>
    [Column("category_id")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Navigation property to Category.
    /// </summary>
    public virtual Category? Category { get; set; }

    /// <summary>
    /// Navigation property to Products collection.
    /// </summary>
    public virtual IList<Product> Products { get; set; } = new List<Product>();
}
