namespace StoreDAL.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// ToDo: add atribute here
public class OrderDetail : BaseEntity
{
    public OrderDetail()
        : base()
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

    // ToDo: add atribute here
    public int OrderId { get; set; }

    // ToDo: add atribute here
    public int ProductId { get; set; }

    // ToDo: add atribute here
    public decimal Price { get; set; }

    // ToDo: add atribute here
    public int ProductAmount { get; set; }

    public CustomerOrder Order { get; set; }

    public Product Product { get; set; }
}
