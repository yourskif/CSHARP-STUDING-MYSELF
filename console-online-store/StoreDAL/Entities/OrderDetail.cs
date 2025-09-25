namespace StoreDAL.Entities;

using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Order line item.
/// </summary>
[Table("order_details")]
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

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_amount")]
    public int ProductAmount { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    public virtual Product? Product { get; set; }
}
