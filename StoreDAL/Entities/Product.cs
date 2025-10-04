namespace StoreDAL.Entities;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Product entity representing individual product items with pricing and stock information.
/// Related to ProductTitle for display name and category information.
/// </summary>
[Table("products")]
public class Product : BaseEntity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Product"/> class.
    /// Parameterless constructor for Entity Framework.
    /// </summary>
    public Product() : base()
    {
        this.Description = string.Empty;
        this.OrderDetails = new List<OrderDetail>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Product"/> class with price only.
    /// Legacy constructor for backward compatibility.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <param name="titleId">Product title identifier.</param>
    /// <param name="manufacturerId">Manufacturer identifier.</param>
    /// <param name="description">Product description.</param>
    /// <param name="price">Unit price.</param>
    public Product(int id, int titleId, int manufacturerId, string description, decimal price)
        : base(id)
    {
        this.TitleId = titleId;
        this.ManufacturerId = manufacturerId;
        this.Description = description ?? string.Empty;
        this.UnitPrice = price;
        this.Stock = 0; // Default stock
        this.OrderDetails = new List<OrderDetail>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Product"/> class with full parameters.
    /// Main constructor for creating products with all necessary data.
    /// </summary>
    /// <param name="id">Product identifier.</param>
    /// <param name="titleId">Product title identifier.</param>
    /// <param name="manufacturerId">Manufacturer identifier.</param>
    /// <param name="description">Product description.</param>
    /// <param name="price">Unit price.</param>
    /// <param name="stock">Stock quantity.</param>
    public Product(int id, int titleId, int manufacturerId, string description, decimal price, int stock)
        : base(id)
    {
        this.TitleId = titleId;
        this.ManufacturerId = manufacturerId;
        this.Description = description ?? string.Empty;
        this.UnitPrice = price;
        this.Stock = stock;
        this.OrderDetails = new List<OrderDetail>();
    }

    /// <summary>
    /// Gets or sets the product title identifier.
    /// References ProductTitle entity for display name and category.
    /// </summary>
    [Column("product_title_id")]
    public int TitleId { get; set; }

    /// <summary>
    /// Gets or sets the manufacturer identifier.
    /// References Manufacturer entity for brand information.
    /// </summary>
    [Column("manufacturer_id")]
    public int ManufacturerId { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// Stored as decimal for precise monetary calculations.
    /// </summary>
    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the product description.
    /// Additional details about the product for customers.
    /// </summary>
    [Column("comment")]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the stock quantity available.
    /// Current inventory level for this product.
    /// </summary>
    [Column("stock_qty")]
    public int Stock { get; set; }

    // Navigation Properties

    /// <summary>
    /// Gets or sets the related product title.
    /// Contains display name and category information.
    /// </summary>
    [ForeignKey("TitleId")]
    public ProductTitle? Title { get; set; }

    /// <summary>
    /// Gets or sets the related manufacturer.
    /// Contains brand and company information.
    /// </summary>
    [ForeignKey("ManufacturerId")]
    public Manufacturer? Manufacturer { get; set; }

    /// <summary>
    /// Gets or sets the collection of order details that reference this product.
    /// Used for tracking product sales history.
    /// </summary>
    public virtual IList<OrderDetail> OrderDetails { get; set; }
}
