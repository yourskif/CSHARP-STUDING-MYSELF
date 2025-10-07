namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Product entity with stock and reservation counters.
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

    [Column("product_title_id")]
    [Required]
    public int ProductTitleId { get; set; }

    public virtual ProductTitle? Title { get; set; }

    [Column("manufacturer_id")]
    [Required]
    public int ManufacturerId { get; set; }

    public virtual Manufacturer? Manufacturer { get; set; }

    [Column("description")]
    public string Description { get; set; } = string.Empty;

    [Column("unit_price")]
    [Required]
    public decimal UnitPrice { get; set; }

    [Column("stock_quantity")]
    public int StockQuantity { get; set; }

    [Column("reserved_quantity")]
    public int ReservedQuantity { get; set; }

    /// <summary>
    /// Gets computed availability (Stock - Reserved), not mapped to DB.
    /// </summary>
    [NotMapped]
    public int AvailableQuantity => this.StockQuantity - this.ReservedQuantity;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
