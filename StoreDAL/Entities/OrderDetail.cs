namespace StoreDAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("customer_order_details")]
public class OrderDetail : BaseEntity
{
    public OrderDetail() : base()
    {
    }

    public OrderDetail(int id, int orderId, int productId, decimal price, int amount)
        : base(id)
    {
        this.OrderId = orderId;
        this.ProductId = productId;
        this.Price = price;
        this.ProductAmount = amount;
    }

    [Column("customer_order_id")]
    public int OrderId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("price")]
    public decimal Price { get; set; }

    [Column("product_amount")]
    public int ProductAmount { get; set; }

    [ForeignKey("OrderId")]
    public CustomerOrder Order { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}
