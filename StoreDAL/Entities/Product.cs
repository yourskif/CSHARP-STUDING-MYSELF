// Path: console-online-store/StoreDAL/Entities/Product.cs
namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Product entity with stock and reservation counters.
/// Table structure matches TZ diagram with extensions for inventory management.
/// </summary>
[Table("products")]
public class Product : BaseEntity
{
    public Product()
        : base()
    {
        this.ReservedQuantity = 0;
        this.StockQuantity = 0;
    }

    public Product(int id, int productTitleId, int manufacturerId, string description, decimal unitPrice)
        : base(id)
    {
        this.ProductTitleId = productTitleId;
        this.ManufacturerId = manufacturerId;
        this.Description = description;
        this.UnitPrice = unitPrice;
        this.StockQuantity = 0;
        this.ReservedQuantity = 0;
    }

    public Product(int id, int productTitleId, int manufacturerId, string description, decimal unitPrice, int stockQuantity)
        : base(id)
    {
        this.ProductTitleId = productTitleId;
        this.ManufacturerId = manufacturerId;
        this.Description = description;
        this.UnitPrice = unitPrice;
        this.StockQuantity = stockQuantity;
        this.ReservedQuantity = 0;
    }

    /// <summary>
    /// Gets or sets the product title ID (foreign key).
    /// Database column: product_title_id
    /// </summary>
    [Column("product_title_id")]
    [Required]
    public int ProductTitleId { get; set; }

    /// <summary>
    /// Navigation property to ProductTitle.
    /// </summary>
    public virtual ProductTitle? Title { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer ID (foreign key).
    /// Database column: manufacturer_id
    /// </summary>
    [Column("manufacturer_id")]
    [Required]
    public int ManufacturerId { get; set; }

    /// <summary>
    /// Navigation property to Manufacturer.
    /// </summary>
    public virtual Manufacturer? Manufacturer { get; set; }

    /// <summary>
    /// Gets or sets the product description/comment.
    /// Database column: comment (per TZ diagram)
    /// </summary>
    [Column("comment")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unit price.
    /// Database column: unit_price
    /// </summary>
    [Column("unit_price")]
    [Required]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the total stock quantity.
    /// Database column: stock_quantity (extension for inventory management)
    /// </summary>
    [Column("stock_quantity")]
    public int StockQuantity { get; set; }

    /// <summary>
    /// Gets or sets the reserved quantity (items in open orders).
    /// Database column: reserved_quantity (extension for inventory management)
    /// </summary>
    [Column("reserved_quantity")]
    public int ReservedQuantity { get; set; }

    /// <summary>
    /// Gets computed availability (Stock - Reserved), not mapped to DB.
    /// </summary>
    [NotMapped]
    public int AvailableQuantity => this.StockQuantity - this.ReservedQuantity;

    /// <summary>
    /// Navigation property to OrderDetails collection.
    /// </summary>
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
