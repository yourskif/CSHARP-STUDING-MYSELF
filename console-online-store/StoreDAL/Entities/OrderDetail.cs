namespace StoreDAL.Entities;

using System.ComponentModel.DataAnnotations.Schema;

[Table("order_details")]
public class OrderDetail : BaseEntity
{
    public OrderDetail() : base()
    {
    }

    // Keep exactly this constructor: BLL passes named arg "amount"
    public OrderDetail(int id, int orderId, int productId, int amount, decimal price)
        : base(id)
    {
        this.CustomerOrderId = orderId;
        this.ProductId = productId;
        this.Quantity = amount;
        this.UnitPrice = price;
    }

    [Column("order_id")]
    public int CustomerOrderId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_amount")]
    public int Quantity { get; set; }

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    // Aliases for BLL
    [NotMapped]
    public int OrderId
    {
        get => this.CustomerOrderId;
        set => this.CustomerOrderId = value;
    }

    [NotMapped]
    public int ProductAmount
    {
        get => this.Quantity;
        set => this.Quantity = value;
    }

    [NotMapped]
    public decimal Price
    {
        get => this.UnitPrice;
        set => this.UnitPrice = value;
    }

    [ForeignKey("CustomerOrderId")]
    public CustomerOrder CustomerOrder { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
}
