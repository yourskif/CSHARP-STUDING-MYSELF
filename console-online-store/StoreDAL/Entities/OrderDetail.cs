// Path: console-online-store/StoreDAL/Entities/OrderDetail.cs
namespace StoreDAL.Entities;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Order line item entity.
/// Table structure matches TZ diagram exactly.
/// </summary>
[Table("customer_order_details")]
public class OrderDetail : BaseEntity
{
    public OrderDetail()
        : base()
    {
    }

    public OrderDetail(int id, int orderId, int productId, int productAmount, decimal price)
        : base(id)
    {
        this.OrderId = orderId;
        this.ProductId = productId;
        this.ProductAmount = productAmount;
        this.Price = price;
    }

    /// <summary>
    /// Gets or sets the order ID (foreign key).
    /// Database column: customer_order_id (per TZ diagram)
    /// </summary>
    [Column("customer_order_id")]
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the product ID (foreign key).
    /// Database column: product_id
    /// </summary>
    [Column("product_id")]
    public int ProductId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of product in this order line.
    /// Database column: product_amount
    /// </summary>
    [Column("product_amount")]
    public int ProductAmount { get; set; }

    /// <summary>
    /// Gets or sets the price at time of order.
    /// Database column: price
    /// </summary>
    [Column("price")]
    public decimal Price { get; set; }

    /// <summary>
    /// Navigation property to Product.
    /// </summary>
    public virtual Product? Product { get; set; }
}
