namespace StoreBLL.Models
{
    public class OrderDetailModel : AbstractModel
    {
        public OrderDetailModel() : base()
        {
        }

        public OrderDetailModel(int id, int orderId, int productId, decimal price, int amount)
            : base(id)
        {
            this.OrderId = orderId;
            this.ProductId = productId;
            this.Price = price;
            this.ProductAmount = amount;
        }

        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int ProductAmount { get; set; }

        public override string ToString()
        {
            return $"Id: {this.Id}, OrderId: {this.OrderId}, ProductId: {this.ProductId}, Price: {this.Price:C}, Amount: {this.ProductAmount}";
        }
    }
}