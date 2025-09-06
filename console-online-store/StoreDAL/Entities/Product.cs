// C:\Users\SK\source\repos\C#\CSHARP-STUDING-MYSELF\console-online-store\StoreDAL\Entities\Product.cs
namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Product row. Prices live in <see cref="UnitPrice"/>, stock in <see cref="Stock"/>.
/// Title text is stored in related <see cref="ProductTitle"/> (fallback: <see cref="Description"/>).
/// </summary>
[Table("products")]
public class Product : BaseEntity
{
    public Product() : base() { }

    public Product(int id, int titleId, int manufacturerId, string description, decimal price, int stock)
        : base(id)
    {
        this.TitleId = titleId;
        this.ManufacturerId = manufacturerId;
        this.Description = description;
        this.UnitPrice = price;
        this.Stock = stock;
    }

    [Column("product_title_id")]
    public int TitleId { get; set; }

    [Column("manufacturer_id")]
    public int ManufacturerId { get; set; }

    /// <summary>Unit price in the smallest currency unit.</summary>
    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    /// <summary>Free text fallback/title; used by legacy UI.</summary>
    [Column("comment")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Current stock quantity.</summary>
    [Column("stock_qty")]
    public int Stock { get; set; }

    // Navigation
    [ForeignKey(nameof(TitleId))]
    public ProductTitle? Title { get; set; }

    [ForeignKey(nameof(ManufacturerId))]
    public Manufacturer? Manufacturer { get; set; }

    public virtual IList<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
